
const sexType = {
    0: '男',
    1: '女',
    2: '[未知]'
};

const orderStatusType =
{
    0: '新建',
    1: '成功',
    2: '失败',
    3: '过期'
};

function enumFormatter(cell, row, enumObject) {
    return enumObject[cell];
};

function boolFormatter(cell, row) {
    return cell ? "是" : "否";
};

function linkFormatter(cell, row) {
    return '<a href="/Home/orderList?accountId=' + cell + '" >订单</a>';
}

var UserList = React.createClass({

    render: function () {
        return (
          <BootstrapTable data={this.props.data} striped={true} condensed={true} remote={true} search={true} pagination={true}
                          fetchInfo={{ dataTotalSize: this.props.totalDataSize }}
                          options={{ onSearchChange: this.props.onSearchChange, sizePerPage: this.props.sizePerPage, onPageChange: this.props.onPageChange, sizePerPageList: [5, 10, 20, 50, 100], page: this.props.currentPage, onSizePerPageList: this.props.onSizePerPageList }}>
            <TableHeaderColumn dataField='AccountName' isKey={true}>账号</TableHeaderColumn>
            <TableHeaderColumn dataField='Name'>姓名</TableHeaderColumn>
            <TableHeaderColumn dataField='Sex' dataFormat={ enumFormatter} formatExtraData={ sexType }>性别</TableHeaderColumn>
            <TableHeaderColumn dataField='IsGuest' dataFormat={ boolFormatter}>访客?</TableHeaderColumn>
            <TableHeaderColumn dataField='IdentityNum'>IdentityNum</TableHeaderColumn>
            <TableHeaderColumn dataField='PhoneNum'>手机</TableHeaderColumn>
            <TableHeaderColumn dataField='Email'>邮箱</TableHeaderColumn>
            <TableHeaderColumn dataField='CreatedDateTime'>注册日期</TableHeaderColumn>
            <TableHeaderColumn dataField='LastLoginDateTime'>最后登录日期</TableHeaderColumn>
            <TableHeaderColumn dataField='Id' dataFormat={linkFormatter}>订单</TableHeaderColumn>
          </BootstrapTable>
        );
    }
});

var OrderList = React.createClass({
    render: function () {
        return (
            <BootstrapTable data={this.props.data} condensed={true} striped={true} remote={true} search={true} pagination={true}
                            fetchInfo={{ dataTotalSize: this.props.totalDataSize }}
                            options={{ onSearchChange: this.props.onSearchChange, sizePerPage: this.props.sizePerPage, onPageChange: this.props.onPageChange, sizePerPageList: [5, 10, 20, 50, 100], page: this.props.currentPage, onSizePerPageList: this.props.onSizePerPageList }}>
            <TableHeaderColumn dataField='OrderId' isKey={true}>订单Id</TableHeaderColumn>
            <TableHeaderColumn dataField='AccountName'>账号</TableHeaderColumn>
            <TableHeaderColumn dataField='AppId'>App Id</TableHeaderColumn>
            <TableHeaderColumn dataField='OrderStatus' dataFormat={ enumFormatter} formatExtraData={ orderStatusType }>订单状态</TableHeaderColumn>
            <TableHeaderColumn dataField='RawData'>订单数据</TableHeaderColumn>
            <TableHeaderColumn dataField='CreatedTime'>创建时间</TableHeaderColumn>
            <TableHeaderColumn dataField='CompletedTime'>结束时间</TableHeaderColumn>
            </BootstrapTable>
        );
    }
});

var TableMixin = {
    getUserList: function (keyword, orderby, page, pageSize) {
        $.ajax({
            url: this.props.url,
            data: {
                keyword: keyword,
                orderby: orderby,
                page: page,
                count: pageSize
            },
            dataType: 'json',
            cache: false,
            success: function (data) {
                this.setState({
                    keyword: keyword,
                    orderby: orderby,
                    totalDataSize: data.Total,
                    currentPage: data.Page,
                    sizePerPage: pageSize,
                    data: data.Raws,
                });
            }.bind(this),
            error: function (xhr, status, err) {
                console.error(this.props.url, status, err.toString());
            }.bind(this)
        });
    },
    getInitialState: function () {
        return { totalDataSize: 0, currentPage: 1, sizePerPage: 10, data: [] };
    },
    componentDidMount: function () {
        this.getUserList(this.state.keyword, this.state.orderby, this.state.currentPage, this.state.sizePerPage);
    },
    onPageChange: function (page, sizePerPage) {
        this.getUserList(this.state.keyword, this.state.orderby, page, sizePerPage);
    },
    onSearchChange: function (searchText, colInfos, multiColumnSearch) {
        this.getUserList(searchText, this.state.orderby, this.state.currentPage, this.state.sizePerPage);
    },
    onSizePerPageList: function (sizePerPage) {
        this.getUserList(this.state.keyword, this.state.orderby, this.state.currentPage, sizePerPage);
    }
}

var UserListBox = React.createClass({
    mixins: [TableMixin],
    render: function () {
        return (
            <UserList data={this.state.data}
                      totalDataSize={this.state.totalDataSize}
                      sizePerPage={this.state.sizePerPage}
                      onSearchChange={this.onSearchChange}
                      onPageChange={this.onPageChange}
                      currentPage={this.state.currentPage}
                      onSizePerPageList={this.onSizePerPageList}></UserList>
        );
    }
});

var OrderListBox = React.createClass({
    mixins: [TableMixin],
    render: function () {
        return (
            <OrderList data={this.state.data}
                       totalDataSize={this.state.totalDataSize}
                       sizePerPage={this.state.sizePerPage}
                       onSearchChange={this.onSearchChange}
                       onPageChange={this.onPageChange}
                       currentPage={this.state.currentPage}
                       onSizePerPageList={this.onSizePerPageList}></OrderList>
        );
    }
});
