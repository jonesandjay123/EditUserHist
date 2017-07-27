<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddReport.aspx.cs" Inherits="EditUserHist.AddReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style>
    #inner {
      display: table;
      margin: 0 auto;
    }
</style>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <div id="outer" style="width:100%">


    
    <h3 style="text-align: center;">新增報工紀錄</h3>

    <form id="form1" runat="server">
        <asp:Button OnClick="returnLastPage" runat="server" Text="修改工時" Style="float:right; margin-right:150px" />
         
        <div id="inner">


         <asp:Panel ID="panel" runat="server" DefaultButton="btnSearch">

            <label for="usr">工單號碼:</label>
            <asp:TextBox ID="EnterWO" runat="server" style="width:130px"></asp:TextBox>

            <label for="usr">報工人員:</label>
            <asp:TextBox ID="CreateUser" runat="server" style="width:80px"></asp:TextBox>

             <asp:Button OnClick="printInfo" runat="server" Text="查詢" />
             <br/>
             <hr/>
             <br/>


             <label>工單號碼: </label> <asp:Label ID="searchWO" runat="server" Text="--"></asp:Label>

             <label>姓名: </label> <asp:Label ID="EmpName" runat="server" Text="--"></asp:Label>

             <br/>
             <br/>

             <div>

                <label>工序:</label>
                <asp:DropDownList ID="OperSEQ" runat="server"> </asp:DropDownList>


                <label>階段狀態:</label>
                <asp:DropDownList ID="WorkStatus" runat="server"> 
                    <asp:ListItem Value="Production" Selected="True">生產</asp:ListItem>
                    <asp:ListItem Value="Prepare">準備</asp:ListItem> 
                    <asp:ListItem Value="Exception">除外</asp:ListItem>  
                    <asp:ListItem Value="Clean">清理</asp:ListItem> 
                    <asp:ListItem Value="Init">分批</asp:ListItem> 
                </asp:DropDownList>

                 <br/>
                 <br/>

                 <asp:RadioButtonList ID="typeSelect" runat="server" OnSelectedIndexChanged="checkSelection" AutoPostBack="true">
                    <asp:ListItem Value="regular" Selected="True">一般報工</asp:ListItem>
                    <asp:ListItem Value="prePrint">印前報工</asp:ListItem>
                </asp:RadioButtonList>


                 <br/>
                

                <label for="usr">開始時間:</label>
                <asp:TextBox ID="StartTime" runat="server" style="width:150px"></asp:TextBox>

                <label for="usr">結束時間:</label>
                <asp:TextBox ID="EndTime" runat="server" style="width:150px"></asp:TextBox>
                 <br/>
                 <br/>

                 <div runat="server" id="qtyReport">

                <label for="usr">報工數量:</label>
                <asp:TextBox ID="REPORT_OUT_OK_QTY" runat="server" style="width:150px"></asp:TextBox>

                <label for="usr">不良數量:</label>
                <asp:TextBox ID="REPORT_OUT_NG_QTY" runat="server" style="width:150px"></asp:TextBox>
                 <br/>
                 <br/>
                 </div>

                 <div runat="server" id="prePrintReport" Visible="false">

                 <label>印前:</label>
                <asp:DropDownList ID="PreprintSec" runat="server"> 
                    <asp:ListItem Value="PrePrintDone" Selected="True">完工數量</asp:ListItem>
                    <asp:ListItem Value="PrePrintDefect">材料用量</asp:ListItem> 
                </asp:DropDownList>
                <asp:TextBox ID="PREPRINT_QTY" runat="server" style="width:100px"></asp:TextBox>
                </div>
                <asp:Button ID="btnSearch" OnClick="insertData" runat="server" Text="送出" Style="float:right" />

             </div>
         </asp:Panel>
        </div>
    </form>
    </div>
</body>
</html>
