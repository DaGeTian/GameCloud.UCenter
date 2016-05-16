
const sexType = {
    0: '男',
    1: '女',
    2: '[未知]'
};

function enumFormatter(cell, row, enumObject) {
    return enumObject[cell];
};

function boolFormatter(cell, row) {
    return cell ? "是" : "否";
};

var UserList = React.createClass({

    render: function () {
        return (
          <BootstrapTable data={this.props.data} remote={true} search={true} pagination={true}
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
          </BootstrapTable>
        );
    }
});

var UserListBox = React.createClass({
    getUserList: function (keyword, orderby, page, pageSize) {
        $.ajax({
            url: "/api/users",
            data: {
                keyword: keyword,
                orderby: orderby,
                page: page,
                count: pageSize
            },
            dataType: 'json',
            cache: false,
            success: function (data) {
                console.log(data);
                this.setState({
                    keyword: keyword,
                    orderby: orderby,
                    totalDataSize: data.Total,
                    currentPage: data.Page,
                    sizePerPage: pageSize,
                    data: data.Users,
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
    },
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

ReactDOM.render(
          <UserListBox />,
          document.getElementById('userList')
);