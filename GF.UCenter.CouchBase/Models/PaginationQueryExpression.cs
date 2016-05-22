using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.N1QL;
using GF.UCenter.Common;

namespace GF.UCenter.CouchBase.Models
{
    public class PaginationQueryExpression<TEntity> where TEntity : class, IBaseEntity
    {
        public PaginationQueryExpression(int page, int pageSize)
            : this(page, pageSize, null)
        {
        }

        public PaginationQueryExpression(int page, int pageSize, Expression<Func<TEntity, bool>> condition)
        {
            this.Page = page > 0 ? page : 1;
            this.PageSize = pageSize;
            this.AdditionalCondition = condition;
            this.LikeItems = new List<LikeItem>();
            this.OrderByItems = new List<OrderByItem>();
        }

        public int Page { get; private set; }

        public int PageSize { get; private set; }

        public List<LikeItem> LikeItems { get; private set; }

        public List<OrderByItem> OrderByItems { get; private set; }

        public Expression<Func<TEntity, bool>> AdditionalCondition { get; private set; }

        public void AddLikeItem<T>(Expression<Func<TEntity, T>> expression, string value)
        {
            var memberExpression = (MemberExpression)expression.Body;
            this.LikeItems.Add(new LikeItem(memberExpression, value));
        }

        public void AddOrderByItem<T>(Expression<Func<TEntity, T>> expression, OrderByType type)
        {
            var memberExpression = (MemberExpression)expression.Body;
            this.OrderByItems.Add(new OrderByItem(memberExpression, type));
        }

        public void AddOrderByItem(string term, OrderByType type)
        {
            this.OrderByItems.Add(new OrderByItem(term, type));
        }

        public QueryRequest BuildQueryRawRequest(IBucket bucket)
        {
            QueryCommand where = this.BuildWhere();
            var query = $"SELECT {bucket.Name}.* FROM {bucket.Name} where {where.Command}";
            var order = this.BuildOrderBy();
            if (!string.IsNullOrEmpty(order))
            {
                query = $"{query} ORDER BY {order}";
            }

            query = $"{query} LIMIT {this.PageSize} OFFSET {(this.Page - 1) * this.PageSize};";
            var request = new QueryRequest();
            request.Statement(query);
            request.AddPositionalParameter(where.Parameters.Select(p => p.Value).ToArray());

            return request;
        }

        public QueryRequest BuildQueryCountRequest(IBucket bucket)
        {
            QueryCommand where = this.BuildWhere();
            var query = $"SELECT COUNT(1) AS Count FROM {bucket.Name} where {where.Command}";

            var request = new QueryRequest();
            request.Statement(query);
            request.AddPositionalParameter(where.Parameters.ToArray());

            return request;
        }

        private QueryCommand BuildWhere()
        {
            ICollection<QueryParameter> parameters = new List<QueryParameter>();
            string where = $"type='{BaseEntity<TEntity>.DocumentType}'";
            if (this.LikeItems.Count > 0)
            {
                string like = string.Join(
                    " OR ",
                    this.LikeItems.Select(item => $"{item.Expression.Member.Name.FirstCharacterToLower()} LIKE '%{item.Value}%'"));

                where = $"{where} AND({like})";
            }

            if (this.AdditionalCondition != null)
            {
                var translator = new CouchQueryTranslator();
                var command = translator.Translate(this.AdditionalCondition);
                where = $"{where} AND ({command.Command})";
                parameters = command.Parameters;
            }

            return new QueryCommand(where, parameters);
        }

        private string BuildOrderBy()
        {
            if (this.OrderByItems.Count > 0)
            {
                var terms = this.OrderByItems.Select(item => $"{item.Term} {item.Type.ToString()}");
                return "ORDER BY " + string.Join(",", terms);
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public class LikeItem
    {
        public LikeItem(Expression expression, string value)
        {
            this.Expression = (MemberExpression)expression;
            this.Value = value;
        }

        public MemberExpression Expression { get; set; }

        public string Value { get; set; }
    }

    public class OrderByItem
    {
        public OrderByItem(MemberExpression expression, OrderByType type)
            : this(expression.Member.Name, type)
        {
        }

        public OrderByItem(string term, OrderByType type)
        {
            this.Term = term.FirstCharacterToLower();
            this.Type = type;
        }

        public MemberExpression Expression { get; set; }

        public string Term { get; set; }

        public OrderByType Type { get; set; }
    }

    public enum OrderByType
    {
        ASC,
        DESC
    }

    public class CountRaw
    {
        public int Count { get; set; }
    }
}
