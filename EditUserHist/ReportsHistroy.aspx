<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportsHistroy.aspx.cs" Inherits="EditUserHist.ReportsHistroy" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<style>
    #inner {
      display: table;
      margin: 0 auto;
    }

     .smallSize { font-size: x-small } 

</style>


<style>
/* The Modal (background) */
.modal {
    display: none; /* Hidden by default */
    position: fixed; /* Stay in place */
    z-index: 1; /* Sit on top */
    padding-top: 100px; /* Location of the box */
    left: 0;
    top: 0;
    width: 100%; /* Full width */
    height: 100%; /* Full height */
    overflow: auto; /* Enable scroll if needed */
    background-color: rgb(0,0,0); /* Fallback color */
    background-color: rgba(0,0,0,0.4); /* Black w/ opacity */
}

/* Modal Content */
.modal-content {
    position: relative;
    background-color: #fefefe;
    margin: auto;
    padding: 0;
    border: 1px solid #888;
    width: 40%;
    box-shadow: 0 4px 8px 0 rgba(0,0,0,0.2),0 6px 20px 0 rgba(0,0,0,0.19);
    -webkit-animation-name: animatetop;
    -webkit-animation-duration: 0.4s;
    animation-name: animatetop;
    animation-duration: 0.4s
}

/* Add Animation */
@-webkit-keyframes animatetop {
    from {top:-300px; opacity:0} 
    to {top:0; opacity:1}
}

@keyframes animatetop {
    from {top:-300px; opacity:0}
    to {top:0; opacity:1}
}

/* The Close Button */
.close {
    color: white;
    float: right;
    font-size: 28px;
    font-weight: bold;
}

.close:hover,
.close:focus {
    color: #000;
    text-decoration: none;
    cursor: pointer;
}

.modal-header {
    padding: 2px 16px;
    background-color: #5cb85c;
    color: white;
}

.modal-body {padding: 2px 16px;}

.modal-footer {
    padding: 2px 16px;
    background-color: #5cb85c;
    color: white;
}
</style>


<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>報工時間調整</title>

     <!-- Bootstrap core CSS -->
      <LINK REL="stylesheet" TYPE="text/css" HREF="Css\starter-template.css">
      <LINK REL="stylesheet" TYPE="text/css" HREF="Css\bootstrap.min.css">
      <LINK REL="stylesheet" TYPE="text/css" HREF="Css\bootstrap-theme.min.css">
      <LINK REL="stylesheet" TYPE="text/css" HREF="Css\theme.css">

      <%--<link rel="stylesheet" href="http://code.jquery.com/ui/1.10.4/themes/trontastic/jquery-ui.css">--%>
      <%--<script src="http://code.jquery.com/jquery-1.10.2.js"></script>--%>
      <%--<script src="http://code.jquery.com/ui/1.10.4/jquery-ui.js"></script>--%>
      
      <link rel="stylesheet" href="Script/jquery-ui.css"">
      <script type="text/javascript" src="Script/jquery-3.1.1.min.js"></script>
      <script type="text/javascript" src="Script/jquery-ui.js"></script>

      <script type="text/javascript" src="Script/bootstrap.min.js"></script>
      <!-- IE10 viewport hack for Surface/desktop Windows 8 bug -->
      <script type="text/javascript" src="Script/ie10-viewport-bug-workaround.js"></script>


    <!-- Just for debugging purposes. Don't actually copy these 2 lines! -->
    <!--[if lt IE 9]><script src="../../assets/js/ie8-responsive-file-warning.js"></script><![endif]-->
    <script src="Script/ie-emulation-modes-warning.js"></script>

    <!-- HTML5 shim and Respond.js for IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/html5shiv/3.7.2/html5shiv.min.js"></script>
      <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
    <![endif]-->

        <script type="text/javascript">
            function showModal() {
                $("#myModal").modal('show');
            }
        </script>
</head>
<body>
    <div id="outer" style="width:100%">


    <form id="form1" runat="server">

    <%--<asp:Button OnClick="goAddReport" runat="server" Text="新增工時" Style="float:right; margin-right:150px" />--%>

 
    <asp:Panel ID="panel" runat="server" DefaultButton="btnSearch">
    <%--<div class='col-md-6 col-md-offset-4'>--%>
        <div id="inner">
        <h3 style="padding-left: 180px;">報工時間調整</h3>
        <span>登入操作者:&nbsp;&nbsp</span><span id="userName"></span>

       <asp:TextBox ID="loginID" runat="server" style="width:60px" type="hidden"></asp:TextBox>
        <br/>

        <label for="usr">生產批號:</label>
        <asp:TextBox ID="EnterWO" runat="server" style="width:130px"></asp:TextBox>

        <label>工序別:</label>
        <asp:TextBox ID="OPER_SEQ" runat="server" style="width:80px"></asp:TextBox>

        <label>報工人員:</label>
        <asp:TextBox ID="EnterID" runat="server" style="width:100px"></asp:TextBox>
        <br/>

        <label>結束時間:</label>
        <asp:TextBox ID="datepicker" runat="server" style="width:100px"></asp:TextBox>
        <small>(之後的時間)</small><span>&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;</span>

        <label>筆數上限</label>
        <asp:DropDownList ID="maxQueryNum" runat="server"> 
            <asp:ListItem>10</asp:ListItem> 
            <asp:ListItem Selected="True">50</asp:ListItem> 
            <asp:ListItem>100</asp:ListItem> 
            <asp:ListItem>200</asp:ListItem> 
        </asp:DropDownList>
        <span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>

        <asp:Button ID="btnSearch" OnClick="queryHistroy" runat="server" Text="查詢" />



        <br/>

    </div>


	    <br/>
	    <br/>

         <asp:GridView ID="GridView1" runat="server" AllowSorting="true" OnSorting="GridView1_Sorting" AutoGenerateColumns="False" DataKeyNames="WIP_LOT_USER_HIST_SID" CellPadding="4" ForeColor="Black" GridLines="Vertical" HorizontalAlign="Center" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating" OnRowDataBound="GridView1_RowDataBound">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:BoundField DataField="WIP_LOT_USER_HIST_SID" HeaderText="WIP_LOT_USER_HIST_SID" InsertVisible="False" ReadOnly="True" SortExpression="WIP_LOT_USER_HIST_SID" visible="false" />
                <asp:BoundField DataField="LOT" HeaderText="工單號碼" SortExpression="LOT" ItemStyle-Width="130px" ReadOnly="true" />
                <asp:BoundField DataField="OPERATION_CODE" HeaderText="工序" SortExpression="OPERATION_CODE" ItemStyle-Width="80px" ReadOnly="true" />
                <asp:BoundField DataField="LOT_SUB_STATUS_CODE" HeaderText="階段狀態" SortExpression="LOT_SUB_STATUS_CODE" ItemStyle-Width="80px" ReadOnly="true" />
                <asp:BoundField DataField="OPERATION_NAME" HeaderText="工序名稱" SortExpression="OPERATION_NAME" ItemStyle-Width="200px" ReadOnly="true" />
                <asp:BoundField DataField="CREATE_USER" HeaderText="報工人員" SortExpression="CREATE_USER" ItemStyle-Width="75px"><controlstyle Width="75px"></controlstyle></asp:BoundField>
                <%-- 更多有關DataFormatString格式的應用，參考: https://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.boundfield.dataformatstring.aspx  --%>
                <%--<asp:BoundField DataField="REPORT_IN_TIME" HeaderText="開始時間" DataFormatString="{0:yyyy/MM/dd HH:mm:ss}"  SortExpression="REPORT_IN_TIME" ItemStyle-Width="170px" />--%>
                <asp:TemplateField HeaderText="開始時間" SortExpression="REPORT_IN_TIME" ItemStyle-Width="170px" >
                    <ItemTemplate>
                        <asp:Label ID="REPORT_IN_TIME" runat="server" Text='<%# Bind("REPORT_IN_TIME","{0:yyyy/MM/dd HH:mm:ss}") %>'></asp:Label>
                    </ItemTemplate>
                    <%--編輯畫面中的時間格式調整方式: https://stackoverflow.com/questions/20462286/how-can-i-put-date-picker-in-gridview-in-edit-mode-asp-net --%>
                    <EditItemTemplate>
                         <asp:TextBox ClientIDMode="Static" ID="IN_TIME_FIX" runat="server" Text='<%#Bind("REPORT_IN_TIME","{0:yyyy/MM/dd HH:mm:ss}") %>' />
                    </EditItemTemplate>
                </asp:TemplateField>
                
                                
                <%--<asp:BoundField DataField="REPORT_OUT_TIME" HeaderText="結束時間" DataFormatString="{0:yyyy/MM/dd HH:mm:ss}"  SortExpression="REPORT_OUT_TIME" ItemStyle-Width="170px" />--%>
                <asp:TemplateField HeaderText="結束時間" SortExpression="REPORT_IN_TIME" ItemStyle-Width="170px" >
                    <ItemTemplate>
                        <asp:Label ID="REPORT_OUT_TIME" runat="server" Text='<%# Bind("REPORT_OUT_TIME","{0:yyyy/MM/dd HH:mm:ss}") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                         <asp:TextBox ClientIDMode="Static" ID="OUT_TIME_FIX" runat="server" Text='<%#Bind("REPORT_OUT_TIME","{0:yyyy/MM/dd HH:mm:ss}") %>' />
                    </EditItemTemplate>
                </asp:TemplateField>


                <asp:BoundField DataField="REPORT_OUT_OK_QTY" HeaderText="報工數量" SortExpression="REPORT_OUT_OK_QTY" ItemStyle-Width="65px"><controlstyle Width="65px"></controlstyle></asp:BoundField>
                <asp:BoundField DataField="REPORT_OUT_NG_QTY" HeaderText="不良數量" SortExpression="REPORT_OUT_NG_QTY" ItemStyle-Width="65px"><controlstyle Width="65px"></controlstyle></asp:BoundField>
                <asp:BoundField DataField="ZZ_BEFER_PRINT_FINISH_QTY" HeaderStyle-CssClass="smallSize" HeaderText="印前<br />完工數量" SortExpression="ZZ_BEFER_PRINT_FINISH_QTY" ItemStyle-Width="65px" HtmlEncode="false" ><controlstyle Width="70px"></controlstyle></asp:BoundField>
                <asp:BoundField DataField="ZZ_BEFER_PRINT_MATERIAL_QTY" HeaderStyle-CssClass="smallSize" HeaderText="印前<br />材料用量" SortExpression="ZZ_BEFER_PRINT_MATERIAL_QTY" ItemStyle-Width="65px" HtmlEncode="false" ><controlstyle Width="70px"></controlstyle></asp:BoundField>
                <asp:BoundField DataField="REASON_DESC" HeaderText="修改<br />紀錄" SortExpression="REASON_DESC" ItemStyle-Width="35px"  HtmlEncode="false" ReadOnly="true" />
                <asp:CommandField HeaderText="修改" ShowEditButton="True"/>



                <%--<asp:templatefield headertext="修改時間">
                <itemtemplate>
                   <asp:Label ID="REPORT_IN_TIME" runat="server" Text='<%# Bind("REPORT_IN_TIME","{0:yyyy/MM/dd HH:mm:ss}") %>'></asp:Label>
                </itemtemplate>
                </asp:templatefield>--%>



            </Columns>
            <FooterStyle BackColor="#CCCC99" />
            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
            <RowStyle BackColor="#F7F7DE" />
            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#FBFBF2" />
            <SortedAscendingHeaderStyle BackColor="#848384" />
            <SortedDescendingCellStyle BackColor="#EAEAD3" />
            <SortedDescendingHeaderStyle BackColor="#575357" />
        </asp:GridView>

    </asp:Panel>



        <div class="modal fade" id="myModal" role="dialog">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <%--<button type="button" class="close" data-dismiss="modal">&times;</button>--%>
                        <asp:Button runat="server" Text="&times;" OnClick="cancelSend" Style="float: right; background-color: lightcoral" />
                        <h3 class="modal-title">請填寫修改原因</h3>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                        <p>修改報公資訊需具名原因，請填寫於下方欄位中:</p>
                        <asp:TextBox ID="purpose" runat="server" style="width:240px" placeholder="原因..."></asp:TextBox>
                    </div>
                    <div class="modal-footer">
                        <asp:Button runat="server" Text="送出" OnClick="reasonSend" Style="color: black" />
                    </div>
                </div>
            </div>
        </div>



    </form>
    </div>

</body>

<script>
$( function() {
    $.datepicker.regional['zh-TW'] = {
        dayNames: ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"],
        dayNamesMin: ["日", "一", "二", "三", "四", "五", "六"],
        monthNames: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
        monthNamesShort: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
        prevText: "上月",
        nextText: "次月",
        weekHeader: "週"
    };
    $.datepicker.setDefaults($.datepicker.regional["zh-TW"]);
    
    $("#datepicker").datepicker({ dateFormat: 'yy/mm/dd' });


    //如果後的Ajax把沒有進一步把loginUser改掉的話，就會使用"011044[ADMIN]"。
    var loginUser = "011044[ADMIN]";

    $.ajax({
        type: "post",
        async: false,
        data: {
            action: "CheckLogin"
        },
        url: "../handler/Loginhanlder.ashx",
        success: function (msg) {
            var jsonObj = jQuery.parseJSON(msg);
            console.log(jsonObj);
            loginUser = jsonObj.user;
            //loginUser = "020518[王眾慧]"; //測試全域變數用的。

            //如果jsonObj.user是有東西的，就會宣告給loginUser，不然就會繼續用ADMIN
            if (jsonObj.user !== null) {
                loginUser = jsonObj.user;
            }
        }
    });

    loginUserID = loginUser.substring(0, 6); //抓取前6碼(工號)要送進寫入資訊當中。
    $("#userName").text(loginUser);          //印出登入者資訊於畫面左上方。
    $("#loginID").val(loginUserID);          //將抓取前6碼的(工號)偷偷藏在隱藏欄位裡面，讓後端可以抓到!

} );
</script>



</html>
