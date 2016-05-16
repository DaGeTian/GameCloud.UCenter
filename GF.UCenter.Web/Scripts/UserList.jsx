var If = React.createClass({
    render: function () {
        if (this.props.test) {
            return this.props.children;
        }
        else {
            return false;
        }
    }
});

var UserRaw = React.createClass({
    displayName: 'UserRaw',
    render: function () {
        return (
            <tr data-id={this.props.Id}>
                <td>{this.props.AccountName}</td>
                <td>{this.props.Name}</td>
                <td>{this.props.Sex}</td>
                <td>{this.props.IsGuest}</td>
                <td>{this.props.IdentityNum}</td>
                <td>{this.props.PhoneNum}</td>
                <td>{this.props.Email}</td>
                <td>{this.props.CreatedDateTime}</td>
                <td>{this.props.LastLoginDateTime}</td>
            </tr>
        );
    }
});

var UserList = React.createClass({
    render: function () {
        var nodes = this.props.data.map(function (user) {
            return (
                <UserRaw Id={user.Id}
                         AccountName={user.AccountName}
                         Name={user.Name}
                         Sex={user.Sex==0?"女":"男"}
                         IsGuest={user.IsGuest?"是":"否"}
                         IdentityNum={user.IdentityNum}
                         PhoneNum={user.PhoneNum}
                         Email={user.Email}
                         CreatedDateTime={moment(user.CreatedDateTime).format('MM/DD/YYYY HH:mm')}
                         LastLoginDateTime={moment(user.LastLoginDateTime).format('MM/DD/YYYY HH:mm:ss')}>
                </UserRaw>
          );
        });
        return (
<table className="table">
<thead>
<tr>
    <th>用户名</th>
    <th>姓名</th>
    <th>性别</th>
    <th>游客?</th>
    <th>IdentityNum</th>
    <th>手机</th>
    <th>邮箱</th>
    <th>注册日期</th>
    <th>最近登录时间</th>
</tr>
</thead>
<tbody>
    {nodes}
</tbody>
</table>
);
    }
});

var SearchBar = React.createClass({
    render: function () {

        var showPages = this.props.showPages || 10;
        var currentPage = this.props.Page;
        var lastPage = parseInt(this.props.Total / this.props.Count + 0.5, 10);
        var minPage = Math.max(currentPage - showPages / 2, 1);
        var maxPage = Math.min(minPage + showPages, lastPage);
        var pages = [];
        for (var p = minPage; p <= maxPage; p++) {
            pages.push(p);
        }

        return (
            <div className="row">
                <div className="col-sm-12">
                    <div className="form-group">
                        <label for="email">关键字:&nbsp;</label>
                        <input type="text" class="form-control" placeholder="用户名/名称/手机/邮箱D" />
                    </div>
                </div>
                <div className="col-sm-6">
                    <div>Max:{maxPage},Min:{minPage},Current:{currentPage},Last:{lastPage}</div>
                    <ul className="pagination">
                        <li><span>共:{this.props.Total}条记录, 当前显示{this.props.Count}条记录</span></li>
                    </ul>
                </div>
                <div className="col-sm-6 text-right">
                    <ul className="pagination">
                        <If test={currentPage>1 }>
                            <li><a href="#">前一页</a></li>
                        </If>
                        {
                            pages.map(function (p) {
                                if (p != currentPage) {
                                    return (
                                        <li><a href="#">{p}</a></li>
                                    );
                                } else {
                                    return (
                                        <li className="active"><a href="#">{p}</a></li>
                                    )
                                }
                            })
                        }
                        <If test={currentPage<lastPage}>
                            <li><a href="#">后一页</a></li>
                        </If>
                        <If test={lastPage>maxPage}>
                            <li><a href="#">最后一页</a></li>
                        </If>
                    </ul>
                </div>
            </div>
        );
    }
});

var UserBox = React.createClass({
    getInitialState: function () {
        return { data: { Total: 0, Page: 0, Count: 0, Users: [] } };
    },
    componentDidMount: function () {
        $.ajax({
            url: this.props.url,
            dataType: 'json',
            cache: false,
            success: function (data) {
                this.setState({ data: data });
            }.bind(this),
            error: function (xhr, status, err) {
                console.error(this.props.url, status, err.toString());
            }.bind(this)
        });
    },
    render: function () {
        return (
                  <div className="commentBox">
                    <h1>用户列表</h1>
                    <SearchBar Page={this.state.data.Page} Total={this.state.data.Total} Count={this.state.data.Count} />
                    <UserList data={this.state.data.Users} />
                  </div>
                );
    }
});

ReactDOM.render(
          <UserBox url="/api/users?page=8&count=100" />,
          document.getElementById('userList')
);