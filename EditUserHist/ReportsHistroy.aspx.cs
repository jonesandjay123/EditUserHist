using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EditUserHist
{
    public partial class ReportsHistroy : System.Web.UI.Page
    {
        String conString = System.Configuration.ConfigurationManager.ConnectionStrings["WeYuConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void goAddReport(object sender, EventArgs e)
        {
            Response.Redirect("AddReport.aspx", true);
        }


        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            //官方有關Sorting的教學: https://msdn.microsoft.com/zh-tw/library/system.web.ui.webcontrols.gridview.sorting(v=vs.110).aspx
            //Retrieve the table from the session object.
            DataTable dt = Session["TaskTable"] as DataTable; 

            if (dt != null){
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('dt != null!')", true);
                //Sort the data.
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                GridView1.DataSource = Session["TaskTable"];
                GridView1.DataBind();
            }
        }

        private string GetSortDirection(string column)
        {

            // By default, set the sort direction to ascending.
            string sortDirection = "ASC";

            // Retrieve the last column that was sorted.
            string sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null)
            {
                // Check if the same column is being sorted.
                // Otherwise, the default value can be returned.
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;
                    if ((lastDirection != null) && (lastDirection == "ASC"))
                    {
                        sortDirection = "DESC";
                    }
                }
            }

            // Save new values in ViewState.
            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            return sortDirection;
        }


        protected void queryHistroy(object sender, EventArgs e)
        {
            myDBInit();
        }

        protected void myDBInit()
        {
            string idValue = EnterID.Text.Trim();
            string WO = EnterWO.Text.Trim();
            string SEQ = OPER_SEQ.Text.Trim();
            string date = datepicker.Text.Trim();
            string maxNum = maxQueryNum.SelectedValue.ToString();
            string loginUser = loginID.Text.Trim();

            /*
            string sql = "SELECT TOP(" + maxNum + ") * FROM( Select HIST.WIP_LOT_USER_HIST_SID, WIP_LOT_HIST.LOT " +
           ",SUBSTRING(WIP_LOT_HIST.OPERATION_CODE, CHARINDEX('-', WIP_LOT_HIST.OPERATION_CODE) + 1, LEN(WIP_LOT_HIST.OPERATION_CODE)) OPERATION_CODE " +
           ",WIP_LOT_HIST.OPERATION_NAME, HIST.LOT_SUB_STATUS_CODE, HIST.CREATE_USER, HIST.REPORT_IN_TIME, HIST.REPORT_OUT_TIME " +
           ",FLOOR(HIST.REPORT_OUT_OK_QTY) REPORT_OUT_OK_QTY, FLOOR(HIST.REPORT_OUT_NG_QTY) REPORT_OUT_NG_QTY " +
           ",FLOOR(HIST.ZZ_BEFER_PRINT_FINISH_QTY) ZZ_BEFER_PRINT_FINISH_QTY, FLOOR(HIST.ZZ_BEFER_PRINT_MATERIAL_QTY) ZZ_BEFER_PRINT_MATERIAL_QTY, CASE WHEN HIST_LOG.REASON_DESC IS NOT NULL THEN 'V' END AS REASON_DESC " +
           "FROM WIP_LOT_USER_HIST AS HIST  INNER JOIN WIP_LOT_HIST ON HIST.IN_WIP_LOT_HIST_SID = WIP_LOT_HIST.WIP_LOT_HIST_SID " +
           "LEFT JOIN ZZ_UPDATE_WIP_LOT_USER_HIST_LOG HIST_LOG ON HIST.WIP_LOT_USER_HIST_SID = HIST_LOG.WIP_LOT_USER_HIST_SID ) t"+
           " where REPORT_OUT_TIME IS NOT NULL ";
           */
            string sql = "SELECT TOP(" + maxNum + ") * FROM( Select HIST.WIP_LOT_USER_HIST_SID, WIP_LOT_HIST.LOT " +
            ", SUBSTRING(WIP_LOT_HIST.OPERATION_CODE, CHARINDEX('-', WIP_LOT_HIST.OPERATION_CODE) + 1, LEN(WIP_LOT_HIST.OPERATION_CODE)) OPERATION_CODE " +
            ", WIP_LOT_HIST.OPERATION_NAME, " +
            //"HIST.LOT_SUB_STATUS_CODE"
            "Case when HIST.LOT_SUB_STATUS_CODE = 'Prepare' then '準備' when HIST.LOT_SUB_STATUS_CODE = 'Production' then '生產' " +
            "when HIST.LOT_SUB_STATUS_CODE = 'Clean' then '清理' when HIST.LOT_SUB_STATUS_CODE = 'Exception' then '除外' "+
            "when HIST.LOT_SUB_STATUS_CODE = 'Init' then '分批' else HIST.LOT_SUB_STATUS_CODE END as LOT_SUB_STATUS_CODE, "

            +  " HIST.CREATE_USER, HIST.REPORT_IN_TIME, HIST.REPORT_OUT_TIME " +
            ", FLOOR(HIST.REPORT_OUT_OK_QTY) REPORT_OUT_OK_QTY, FLOOR(HIST.REPORT_OUT_NG_QTY) REPORT_OUT_NG_QTY " +
            ", FLOOR(HIST.ZZ_BEFER_PRINT_FINISH_QTY) ZZ_BEFER_PRINT_FINISH_QTY, FLOOR(HIST.ZZ_BEFER_PRINT_MATERIAL_QTY) ZZ_BEFER_PRINT_MATERIAL_QTY " +
            ", CASE WHEN HIST_LOG.REASON_DESC IS NOT NULL THEN 'V' END AS REASON_DESC " +
            " FROM WIP_LOT_USER_HIST AS HIST  INNER JOIN WIP_LOT_HIST ON HIST.IN_WIP_LOT_HIST_SID = WIP_LOT_HIST.WIP_LOT_HIST_SID " +
            " LEFT JOIN ( SELECT r.WIP_LOT_USER_HIST_SID, t.REASON_DESC " +
            " FROM(SELECT MAX(LogSN)AS LogSN, WIP_LOT_USER_HIST_SID FROM ZZ_UPDATE_WIP_LOT_USER_HIST_LOG GROUP BY WIP_LOT_USER_HIST_SID) r " +
            " INNER JOIN ZZ_UPDATE_WIP_LOT_USER_HIST_LOG t ON r.LogSN = t.LogSN ) AS HIST_LOG ON HIST.WIP_LOT_USER_HIST_SID = HIST_LOG.WIP_LOT_USER_HIST_SID ) t " +
            " where REPORT_OUT_TIME IS NOT NULL ";




            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('"+ loginUser + "!')", true); //顯示是否有抓到loginUser

            //如果傳進來的資料是空的處理方式(直接return alert)
            if (String.IsNullOrEmpty(idValue) && String.IsNullOrEmpty(WO) && String.IsNullOrEmpty(SEQ) && String.IsNullOrEmpty(date))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('請輸入查詢條件!')", true);
                return;
            }

            if (!String.IsNullOrEmpty(idValue))
            {
                sql = sql + "AND CREATE_USER = '" + idValue + "' ";
            }
            if (!String.IsNullOrEmpty(SEQ))
            {
                //統一使用模糊查詢
                sql = sql + "AND OPERATION_CODE LIKE '" + SEQ + "%'　";
            }
            if (!String.IsNullOrEmpty(WO))
            {
                //模糊查詢
                if (WO.Substring(WO.Length - 1, 1).Equals("%"))
                {
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('%字樣')", true);
                    sql = sql + "AND LOT LIKE '" + WO + "%'　";
                }
                //一般查詢
                else
                {
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + WO + "')", true);
                    sql = sql + "AND LOT = '" + WO + "'　";
                }
            }
            if (!String.IsNullOrEmpty(date))
            {
                DateTime dt;
                string time;

                try
                {
                    dt = Convert.ToDateTime(date); //DateTime
                    time = dt.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                }
                catch (FormatException e)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('日期格式有誤!')", true);
                    System.Diagnostics.Debug.WriteLine("==========FormatException=========");
                    System.Diagnostics.Debug.WriteLine("update時，日期格式有誤:" + e);
                    return;
                }
                sql = sql + "AND REPORT_OUT_TIME >= '" + time + "' ";
            }
            //最後面加上排序
            sql = sql + " order by LOT, OPERATION_CODE asc";
            
            try {
            
                //連線數據庫。參考: https://dotblogs.com.tw/mis2000lab/2010/10/22/ado_net_gridview_sqldatasource_manual_coding
                SqlDataSource SqlDataSource1 = new SqlDataSource();
                SqlDataSource1.ConnectionString = conString;
                //SqlDataSource1.ConnectionString = "Data Source=192.168.59.222;Initial Catalog=dcmate_dev;Persist Security Info=True;User ID=DCMATE;Password=weyu0401;Min Pool Size=100;Max Pool Size=5000;MultipleActiveResultSets=True";
                SqlDataSource1.SelectCommand = sql;
                SqlDataSource1.DataSourceMode = SqlDataSourceMode.DataSet;

                DataSourceSelectArguments args = new DataSourceSelectArguments();
                DataView dv = (DataView)SqlDataSource1.Select(args);
                //GridView1.DataSource = dv;
                //GridView1.DataBind();
                Session["TaskTable"] = dv;
                GridView1.DataSource = Session["TaskTable"];
                GridView1.DataBind();

                SqlDataSource1.Dispose();
            
            }
            catch (Exception e)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('查詢時，出現錯誤!')", true);
                System.Diagnostics.Debug.WriteLine("==========Exception=========");
                System.Diagnostics.Debug.WriteLine("myDBInit()執行時，出現錯誤:" + e);
            }
            
        }

        protected void reasonSend(object sender, EventArgs e)
        {
            string p = purpose.Text.Trim(); 
            //檢驗是否有輸入原因，沒有輸入原因就直接返回，不給予編輯。
            if (String.IsNullOrEmpty(p)) {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('您沒有輸入原因! 編輯結束...')", true);
                GridView1.EditIndex = -1;
                myDBInit();
                return;
            }

        }
        //編輯結束的X
        protected void cancelSend(object sender, EventArgs e)
        {
            GridView1.EditIndex = -1;
            myDBInit();
            return; 
        }
        

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            myDBInit();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {

            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "showModal();", true);  //跳出Modal
            GridView1.EditIndex = e.NewEditIndex;
            ///////////GridView1.Datasource = datasource;  // here you missing
            //GridView1.DataBind();
            myDBInit();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

            string r = purpose.Text.Trim(); //修改原因
            if (String.IsNullOrEmpty(r)) {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('未填寫修改原因!')", true);
                return;
            }

            DateTime REPORT_IN_TIME, REPORT_OUT_TIME;

            try
            {
                //因為日期是透過EditItemTemplate(而非BoundField)的方式做Edit，所以取值的方式要用下面兩行的做法!
                string IN_TIME_FIX = ((TextBox)GridView1.Rows[e.RowIndex].FindControl("IN_TIME_FIX")).Text;
                string OUT_TIME_FIX = ((TextBox)GridView1.Rows[e.RowIndex].FindControl("OUT_TIME_FIX")).Text;
                //將日期格式的字串轉換為SQL要的DateTime，並且用try catch做FormatException的防堵。
                REPORT_IN_TIME = Convert.ToDateTime(IN_TIME_FIX);
                REPORT_OUT_TIME = Convert.ToDateTime(OUT_TIME_FIX);
            }
            catch (FormatException reason) {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('寫入日期格式不正確!')", true);
                System.Diagnostics.Debug.WriteLine("==========FormatException=========");
                System.Diagnostics.Debug.WriteLine("update時，日期格式有誤:" + reason);
                return;
            }

            
            try
            {

                TextBox CREATE_USER = (TextBox)GridView1.Rows[e.RowIndex].Cells[5].Controls[0];
                TextBox REPORT_OUT_OK_QTY = (TextBox)GridView1.Rows[e.RowIndex].Cells[8].Controls[0];
                TextBox REPORT_OUT_NG_QTY = (TextBox)GridView1.Rows[e.RowIndex].Cells[9].Controls[0];
                TextBox ZZ_BEFER_PRINT_FINISH_QTY = (TextBox)GridView1.Rows[e.RowIndex].Cells[10].Controls[0];
                TextBox ZZ_BEFER_PRINT_MATERIAL_QTY = (TextBox)GridView1.Rows[e.RowIndex].Cells[11].Controls[0];


                //int my_id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);   //====主鍵====
                string my_id = GridView1.DataKeys[e.RowIndex].Values[0].ToString();    //====主鍵====(因為長度過長，已不可以用int形式了)


                //資料庫連線
                SqlDataSource SqlDataSource1 = new SqlDataSource();
                SqlDataSource1.ConnectionString = conString;
                //SqlDataSource1.ConnectionString = "Data Source=192.168.59.222;Initial Catalog=dcmate_dev;Persist Security Info=True;User ID=DCMATE;Password=weyu0401;Min Pool Size=100;Max Pool Size=5000;MultipleActiveResultSets=True";
                //SqlDataSource1.UpdateParameters.Clear();

                /*
                //== 設定SQL指令將會用到的參數 ==
                SqlDataSource1.UpdateParameters.Add("WIP_LOT_USER_HIST_SID", my_id.ToString());  //主鍵
                SqlDataSource1.UpdateParameters.Add("REPORT_IN_TIME", TypeCode.DateTime, REPORT_IN_TIME.ToString());
                SqlDataSource1.UpdateParameters.Add("REPORT_OUT_TIME", TypeCode.DateTime, REPORT_OUT_TIME.ToString());
                SqlDataSource1.UpdateParameters.Add("REPORT_OUT_OK_QTY", REPORT_OUT_OK_QTY.Text.ToString());
                SqlDataSource1.UpdateParameters.Add("REPORT_OUT_NG_QTY", REPORT_OUT_NG_QTY.Text.ToString());
                SqlDataSource1.UpdateParameters.Add("ZZ_BEFER_PRINT_FINISH_QTY", ZZ_BEFER_PRINT_FINISH_QTY.Text.ToString());
                SqlDataSource1.UpdateParameters.Add("ZZ_BEFER_PRINT_MATERIAL_QTY", ZZ_BEFER_PRINT_MATERIAL_QTY.Text.ToString());

                //下面的寫法也可以！
                //  SqlDataSource1.UpdateParameters.Add(new Parameter("id",TypeCode.Int32));
                //  SqlDataSource1.UpdateParameters["id"].DefaultValue= my_id.ToString();
                SqlDataSource1.UpdateCommand = "UPDATE WIP_LOT_USER_HIST SET REPORT_IN_TIME = @REPORT_IN_TIME, REPORT_OUT_TIME = @REPORT_OUT_TIME , [REPORT_OUT_OK_QTY] = @REPORT_OUT_OK_QTY, [REPORT_OUT_NG_QTY] = @REPORT_OUT_NG_QTY, [ZZ_BEFER_PRINT_FINISH_QTY]= @ZZ_BEFER_PRINT_FINISH_QTY, [ZZ_BEFER_PRINT_MATERIAL_QTY]= @ZZ_BEFER_PRINT_MATERIAL_QTY  WHERE [WIP_LOT_USER_HIST_SID] = @WIP_LOT_USER_HIST_SID";
                SqlDataSource1.Update();  //執行SQL指令--Update

                SqlDataSource1.Dispose();
                */

                SqlDataSource1.UpdateCommand = "usp_WIP_LOT_USER_HIST_Update";
                SqlDataSource1.UpdateCommandType = SqlDataSourceCommandType.StoredProcedure;

                //SqlDataSource1.UpdateParameters.Add("WIP_LOT_USER_HIST_SID", "123456789");  //刻意使用錯的ID，測試StoredProcedure回傳碼
                SqlDataSource1.UpdateParameters.Add("WIP_LOT_USER_HIST_SID", my_id.ToString());  //主鍵
                SqlDataSource1.UpdateParameters.Add("REPORT_IN_TIME", TypeCode.DateTime, REPORT_IN_TIME.ToString());
                SqlDataSource1.UpdateParameters.Add("REPORT_OUT_TIME", TypeCode.DateTime, REPORT_OUT_TIME.ToString());
                SqlDataSource1.UpdateParameters.Add("REPORT_OUT_OK_QTY", REPORT_OUT_OK_QTY.Text.ToString());
                SqlDataSource1.UpdateParameters.Add("REPORT_OUT_NG_QTY", REPORT_OUT_NG_QTY.Text.ToString());
                SqlDataSource1.UpdateParameters.Add("ZZ_BEFER_PRINT_FINISH_QTY", ZZ_BEFER_PRINT_FINISH_QTY.Text.ToString());
                SqlDataSource1.UpdateParameters.Add("ZZ_BEFER_PRINT_MATERIAL_QTY", ZZ_BEFER_PRINT_MATERIAL_QTY.Text.ToString());
                SqlDataSource1.UpdateParameters.Add("CREATE_USER", CREATE_USER.Text.ToString());
                SqlDataSource1.UpdateParameters.Add("UPDATE_USER", loginID.Text.Trim());  //從前端的隱藏欄位中抓出來的登入者資訊。
                SqlDataSource1.UpdateParameters.Add("REASON", r.ToString());  //修改原因
    


                int retID = SqlDataSource1.UpdateParameters.Add("RetCode", TypeCode.String, "0");
                int x = SqlDataSource1.Update();


                /*
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('回傳碼: " + retID + " " + x + " ')", true);
                SqlDataSource1.SelectCommand = "SELECT @RetCode = @@IDENTITY";
                if (SqlDataSource1.SelectParameters.Count == 0) {
                    SqlDataSource1.SelectParameters.Add("p", DbType.String, x);
                }
                SqlDataSource1.SelectParameters["p"].DefaultValue = x;

                //int x = (int)SqlDataSource1.SelectParameters["@RetCode"].Value;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('回傳碼: " + retID +" "+ x +" ')", true);
                //SqlParameter retID = SqlDataSource1.SelectParameters.Add("@RetCode", SqlDbType.Int);
                */

                //====更新完成後，離開編輯模式 ====

                GridView1.EditIndex = -1;

                //myDBInit(); //--改成更新完後，直接select對應SID的做展示。
                /*
                string sql = "SELECT * FROM( Select HIST.WIP_LOT_USER_HIST_SID, WIP_LOT_HIST.LOT " +
                ",SUBSTRING(WIP_LOT_HIST.OPERATION_CODE, CHARINDEX('-', WIP_LOT_HIST.OPERATION_CODE) + 1, LEN(WIP_LOT_HIST.OPERATION_CODE)) OPERATION_CODE " +
                ",WIP_LOT_HIST.OPERATION_NAME, HIST.LOT_SUB_STATUS_CODE, HIST.CREATE_USER, HIST.REPORT_IN_TIME, HIST.REPORT_OUT_TIME " +
                ",FLOOR(HIST.REPORT_OUT_OK_QTY) REPORT_OUT_OK_QTY, FLOOR(HIST.REPORT_OUT_NG_QTY) REPORT_OUT_NG_QTY " +
                ",FLOOR(HIST.ZZ_BEFER_PRINT_FINISH_QTY) ZZ_BEFER_PRINT_FINISH_QTY, FLOOR(HIST.ZZ_BEFER_PRINT_MATERIAL_QTY) ZZ_BEFER_PRINT_MATERIAL_QTY, CASE WHEN HIST_LOG.REASON_DESC IS NOT NULL THEN 'V' END AS REASON_DESC " +
                "FROM WIP_LOT_USER_HIST AS HIST  INNER JOIN WIP_LOT_HIST  ON HIST.IN_WIP_LOT_HIST_SID = WIP_LOT_HIST.WIP_LOT_HIST_SID " +
                "LEFT JOIN ZZ_UPDATE_WIP_LOT_USER_HIST_LOG HIST_LOG ON HIST.WIP_LOT_USER_HIST_SID = HIST_LOG.WIP_LOT_USER_HIST_SID) t where WIP_LOT_USER_HIST_SID = '" + my_id.ToString() + "' ";
                */

                string sql = "SELECT * FROM( Select HIST.WIP_LOT_USER_HIST_SID, WIP_LOT_HIST.LOT " +
                ", SUBSTRING(WIP_LOT_HIST.OPERATION_CODE, CHARINDEX('-', WIP_LOT_HIST.OPERATION_CODE) + 1, LEN(WIP_LOT_HIST.OPERATION_CODE)) OPERATION_CODE " +
                ", WIP_LOT_HIST.OPERATION_NAME, HIST.LOT_SUB_STATUS_CODE, HIST.CREATE_USER, HIST.REPORT_IN_TIME, HIST.REPORT_OUT_TIME " +
                ", FLOOR(HIST.REPORT_OUT_OK_QTY) REPORT_OUT_OK_QTY, FLOOR(HIST.REPORT_OUT_NG_QTY) REPORT_OUT_NG_QTY " +
                ", FLOOR(HIST.ZZ_BEFER_PRINT_FINISH_QTY) ZZ_BEFER_PRINT_FINISH_QTY, FLOOR(HIST.ZZ_BEFER_PRINT_MATERIAL_QTY) ZZ_BEFER_PRINT_MATERIAL_QTY " +
                //", CASE WHEN HIST_LOG.REASON_DESC IS NOT NULL THEN 'V' END AS REASON_DESC " +
                ", HIST_LOG.REASON_DESC " +
                " FROM WIP_LOT_USER_HIST AS HIST  INNER JOIN WIP_LOT_HIST ON HIST.IN_WIP_LOT_HIST_SID = WIP_LOT_HIST.WIP_LOT_HIST_SID " +
                " LEFT JOIN ( SELECT r.WIP_LOT_USER_HIST_SID, t.REASON_DESC " +
                " FROM(SELECT MAX(LogSN)AS LogSN, WIP_LOT_USER_HIST_SID FROM ZZ_UPDATE_WIP_LOT_USER_HIST_LOG GROUP BY WIP_LOT_USER_HIST_SID) r " +
                " INNER JOIN ZZ_UPDATE_WIP_LOT_USER_HIST_LOG t ON r.LogSN = t.LogSN ) AS HIST_LOG ON HIST.WIP_LOT_USER_HIST_SID = HIST_LOG.WIP_LOT_USER_HIST_SID ) t " +
                " where WIP_LOT_USER_HIST_SID = '" + my_id.ToString() + "' ";

                SqlDataSource1.SelectCommand = sql;
                SqlDataSource1.DataSourceMode = SqlDataSourceMode.DataSet;

                DataSourceSelectArguments args = new DataSourceSelectArguments();
                DataView dv = (DataView)SqlDataSource1.Select(args);
                GridView1.DataSource = dv;
                GridView1.DataBind();

                SqlDataSource1.Dispose();
            
            }
            catch (Exception)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('GridView1_RowUpdating時，出現錯誤!')", true);
                return;
            }
            

        }


        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('RowDataBound!')", true);
            //讓編輯時，enter暫時失效
            e.Row.Attributes.Add("onkeypress", "javascript:if (event.keyCode == 13){}");
            //if (e.Row.HasAttributes) {
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('RowDataBound!')", true);
            //}

        }


    }
}