using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EditUserHist
{
    public partial class AddReport : System.Web.UI.Page
    {

        String conString = System.Configuration.ConfigurationManager.ConnectionStrings["WeYuConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void returnLastPage(object sender, EventArgs e)
        {
            Response.Redirect("ReportsHistroy.aspx", true);
        }

        protected void checkSelection(object sender, EventArgs e)
        {
            string s = typeSelect.SelectedIndex.ToString();

            if (s.Equals("1"))
            {
                qtyReport.Visible = false;
                prePrintReport.Visible = true;
            }
            else {  
                prePrintReport.Visible = false;
                qtyReport.Visible = true;
            }

        }


        protected void printInfo(object sender, EventArgs e)
        {
            string WO = EnterWO.Text.Trim();
            string USER = CreateUser.Text.Trim();

            //如果傳進來的資料是空的處理方式(直接return alert)
            if (String.IsNullOrEmpty(WO))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('沒有輸入工單!')", true);
                return;
            }
            if (String.IsNullOrEmpty(USER))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('沒有輸入人員功號!')", true);
                return;
            }

            


            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('回傳碼: " + WO + " " + USER + " ')", true);
            
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = conString;
            SqlDataReader reader = null;


            try
            {
                //---查工單是否存在------------------
                string sql = "select LOT from WIP_LOT_HIST where LOT = '" + WO+"' ";
                SqlCommand command = new SqlCommand(sql, conn);
                command.Connection.Open();
                reader = command.ExecuteReader();

                //如果工單有存在
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        searchWO.Text = reader[0].ToString();  //就把工單顯示在searchWO
                    }
                }
                //如果沒有資料
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('不存在的工單!')", true);
                    return;
                }
                command.Connection.Close();

                //---查工序----------------
                //先偵測是否用的是子批(有.001、.002的)
                if (WO.Contains('.'))
                {
                    WO = WO.Substring(0, WO.IndexOf('.')); //如果工單號碼是11030018821.001會被改成11030018821   
                }
                sql = "select SAP_SEQ from V_BAS_ROUTE_OPER where  ROUTE_CODE= '" + WO + "' order by SAP_SEQ desc";
                command = new SqlCommand(sql, conn);
                command.Connection.Open();
                reader = command.ExecuteReader();

                //如果有資料
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string sequence = reader[0].ToString();
                        OperSEQ.Items.Insert(0, new ListItem(sequence, sequence));
                    }
                }
                //如果沒有資料
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('查不到工序!')", true);
                    return;
                }
                command.Connection.Close();


                //---查人員工號----------------
                sql = "select USER_NAME, EMP_NO from SEC_USER where EMP_NO = '" + USER + "' ";
                command = new SqlCommand(sql, conn);
                command.Connection.Open();
                reader = command.ExecuteReader();

                //如果有資料
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //Console.WriteLine(String.Format("{0}, {1}", reader[0], reader[1]));
                        EmpName.Text = reader[0].ToString() + "-" + reader[1].ToString();
                    }
                }
                //如果沒有資料
                else {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('查不到人員帳號!')", true);
                }
                command.Connection.Close();

                //如果前面所有條件都通過，表示查詢過關，這時候就貼心的幫忙長出下面四個預填資料。
                //載入時，就先填寫時間。
                StartTime.Text = DateTime.Now.ToString("yyyy/MM/dd 00:00:00");
                EndTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                //載入時，就先填寫預設數量。
                REPORT_OUT_OK_QTY.Text = "0";
                REPORT_OUT_NG_QTY.Text = "0";
                PREPRINT_QTY.Text = "0";

            }
            catch (SqlException excep)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('查詢人員工號時出現錯誤!')", true);
                System.Diagnostics.Debug.WriteLine("==========Exception=========");
                System.Diagnostics.Debug.WriteLine("myDBInit()執行時，出現錯誤:" + excep);
            }


            finally
            {
                // Always call Close when done reading.
                reader.Close();
            }
            
        }

        protected void insertData(object sender, EventArgs e)
        {
            string LOT = searchWO.Text.Trim();
            string USER = EmpName.Text.Trim();
            USER = USER.Substring(USER.Length - 6);
            string SEQ = OperSEQ.SelectedValue.Trim();
            string STATUS = WorkStatus.SelectedValue.Trim();
            string Start = StartTime.Text.Trim();
            string End = EndTime.Text.Trim();
            string OK_QTY = REPORT_OUT_OK_QTY.Text.Trim();
            string NG_QTY = REPORT_OUT_NG_QTY.Text.Trim();
            string PRINT_FIN_QTY = PREPRINT_QTY.Text.Trim();


            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('SEQ: "+ SEQ + " ')", true);

            if (LOT == null || LOT == String.Empty || USER == null || USER == String.Empty || SEQ == null || SEQ == String.Empty ||
                Start == String.Empty || Start == null || End == String.Empty || End == null || OK_QTY == String.Empty || OK_QTY == null || NG_QTY == String.Empty || NG_QTY == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('資料不齊全!')", true);
                return;
            }


            DateTime REPORT_IN_TIME, REPORT_OUT_TIME;

            try
            {
                //將日期格式的字串轉換為SQL要的DateTime，並且用try catch做FormatException的防堵。
                REPORT_IN_TIME = Convert.ToDateTime(Start);
                REPORT_OUT_TIME = Convert.ToDateTime(End);

            }
            catch (FormatException reason)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('寫入日期格式不正確!')", true);
                System.Diagnostics.Debug.WriteLine("==========FormatException=========");
                System.Diagnostics.Debug.WriteLine("update時，日期格式有誤:" + reason);
                return;
            }



            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('回傳碼: " + WO + " " + SEQ + " " + STATUS + " " + USER + " " + REPORT_IN_TIME + " " + REPORT_OUT_TIME + " " + OK_QTY + " " + NG_QTY + "  " + PRINT_FIN_QTY + "  " + PRINT_MTL_QTY + " ')", true);
            if (STATUS == "Prepare") {

                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = conString;
                SqlDataReader reader = null;
                /*
                try
                {
                */
                    //---查工單是否存在------------------
                    string sql = "select LOT_SID, LOT_STATUS_SID, WO, CUR_OPERATION_LINK_SID as OPERATION_LINK_SID from WIP_LOT where LOT = '" + LOT + "' ";
                    SqlCommand command = new SqlCommand(sql, conn);
                    command.Connection.Open();
                    reader = command.ExecuteReader();

                    string LOT_SID = null, ALIAS_LOT1 = "", ALIAS_LOT2 = "", LOT_STATUS_SID = null, 
                    WO = null, OPERATION_LINK_SID = null;
                    while (reader.Read()){
                        LOT_SID = reader[0].ToString();
                        LOT_STATUS_SID = reader[1].ToString();
                        WO = reader[2].ToString();
                        OPERATION_LINK_SID = reader[3].ToString();
                    }
                    command.Connection.Close();

                    sql = "select LOT_STATUS_CODE from LOT_STATUS where LOT_STATUS_SID ='" + LOT_STATUS_SID + "' ";
                    command = new SqlCommand(sql, conn);
                    command.Connection.Open();
                    reader = command.ExecuteReader();

                    string LOT_STATUS_CODE = null;
                    while (reader.Read())
                    {
                        LOT_STATUS_CODE = reader[0].ToString();
                    }
                    command.Connection.Close();

                    sql = "select PRE_LOT_STATUS_SID, PRE_LOT_STATUS_CODE, LOT_QTY, TOTAL_DEFECT_QTY, TOTAL_USER_COUNT from WIP_LOT_HIST where LOT = '" + LOT + "' AND LOT_STATUS_SID = '" + LOT_STATUS_SID + "' ";
                    command = new SqlCommand(sql, conn);
                    command.Connection.Open();
                    reader = command.ExecuteReader();

                    string PRE_LOT_STATUS_SID = null, PRE_LOT_STATUS_CODE= null, LOT_QTY = null, TOTAL_DEFECT_QTY = "0", TOTAL_USER_COUNT = null;
                    while (reader.Read())
                    {
                        PRE_LOT_STATUS_SID = reader[0].ToString();
                        PRE_LOT_STATUS_CODE = reader[1].ToString();
                        LOT_QTY = reader[2].ToString();
                        TOTAL_DEFECT_QTY = reader[3].ToString();
                        TOTAL_USER_COUNT = reader[4].ToString();
                    }
                    command.Connection.Close();

                    sql = "select WO_SID, PART_SID, PART_NO, FACTORY_SID, ROUTE_SID from  WOR_MASTER  where WO ='" + WO +"' ";
                    command = new SqlCommand(sql, conn);
                    command.Connection.Open();
                    reader = command.ExecuteReader();

                    string WO_SID = null, PART_SID = null, PART_NO = null, FACTORY_SID = null, ROUTE_SID = null;
                    while (reader.Read())
                    {
                        WO_SID = reader[0].ToString();
                        PART_SID = reader[1].ToString();
                        PART_NO = reader[2].ToString();
                        FACTORY_SID = reader[3].ToString();
                        ROUTE_SID = reader[4].ToString();
                    }
                    command.Connection.Close();
           
                    sql = "select OPERATION_SID, OPERATION_CODE, OPERATION_NAME, SEQ as OPERATION_SEQ from V_BAS_ROUTE_OPER where ROUTE_CODE = '" + WO + "' and SAP_SEQ = '"+ SEQ + "' ";
                    command = new SqlCommand(sql, conn);
                    command.Connection.Open();
                    reader = command.ExecuteReader();

                    string OPERATION_SID = null, OPERATION_CODE=null, OPERATION_NAME=null, OPERATION_SEQ=null, OPERATION_FINISH="N";
                    while (reader.Read())
                    {
                        OPERATION_SID = reader[0].ToString();
                        OPERATION_CODE = reader[1].ToString();
                        OPERATION_NAME = reader[2].ToString();
                        OPERATION_SEQ = reader[3].ToString();
                    }
                    command.Connection.Close();

                    sql = "select FACTORY_CODE, FACTORY_NAME from BAS_FACTORY  where FACTORY_SID = '"+ FACTORY_SID  + "' ";
                    command = new SqlCommand(sql, conn);
                    command.Connection.Open();
                    reader = command.ExecuteReader();

                    string FACTORY_CODE = null, FACTORY_NAME = null;
                    while (reader.Read())
                    {
                        FACTORY_CODE = reader[0].ToString();
                        FACTORY_NAME = reader[1].ToString();
                    }
                    command.Connection.Close();

                    sql = "select SHIFT_SID, WORKGROUP_SID from SEC_USER  where ACCOUNT_NO = '" + USER + "' ";
                    command = new SqlCommand(sql, conn);
                    command.Connection.Open();
                    reader = command.ExecuteReader();

                    string SHIFT_SID = null, WORKGROUP_SID = null;
                    while (reader.Read())
                    {
                        SHIFT_SID = reader[0].ToString();
                        WORKGROUP_SID = reader[1].ToString();
                    }
                    command.Connection.Close();



                    string ACTION_CODE = "CHECK_IN", CONTROL_MODE = "", LOT_SUB_STATUS_CODE = "Prepare", 
                    NEXT_OPERATION_SID = null, NEXT_OPERATION_CODE = null, NEXT_OPERATION_NAME = null, NEXT_OPERATION_SEQ = null;
                    //直接抓目前的程式名稱
                    string INPUT_FORM_NAME = this.Page.ToString().Substring(4, this.Page.ToString().Substring(4).Length - 5) + ".aspx"; 
                    string LOT_QTY1 = "0", LOT_QTY2 = "0", LOCATION = "";

                /*
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('走準備:! LOT_SID:" + LOT_SID + " LOT:" + LOT +
                        " ALIAS_LOT1:" + ALIAS_LOT1 + " ALIAS_LOT2" + ALIAS_LOT2 +
                        " LOT_STATUS_SID:" + LOT_STATUS_SID + "  LOT_STATUS_CODE:" + LOT_STATUS_CODE +
                        " PRE_LOT_STATUS_SID:" + PRE_LOT_STATUS_SID + " PRE_LOT_STATUS_CODE:" + PRE_LOT_STATUS_CODE +
                        " WO_SID:" + WO_SID + " WO:" + WO + " OPERATION_LINK_SID:" + OPERATION_LINK_SID + " OPERATION_SID:" + OPERATION_SID +
                        " OPERATION_CODE:" + OPERATION_CODE + " OPERATION_NAME:" + OPERATION_NAME + " OPERATION_SEQ:" + OPERATION_SEQ + " OPERATION_FINISH:" + OPERATION_FINISH +
                        " PART_SID:" + PART_SID + " PART_NO:" + PART_NO + " LOT_QTY:" + LOT_QTY + " TOTAL_OK_QTY:" + OK_QTY + " TOTAL_NG_QTY:" + NG_QTY +
                        " TOTAL_DEFECT_QTY:" + TOTAL_DEFECT_QTY + " TOTAL_USER_COUNT:" + TOTAL_USER_COUNT + " FACTORY_SID:" + FACTORY_SID +
                        " FACTORY_CODE:" + FACTORY_CODE + " FACTORY_NAME:" + FACTORY_NAME + " ACTION_CODE:" + ACTION_CODE + " CONTROL_MODE:" + CONTROL_MODE + " INPUT_FORM_NAME:" + INPUT_FORM_NAME +
                        " CREATE_USER:" + USER + " CREATE_TIME:" + DateTime.Now + " REPORT_TIME:" + DateTime.Now + " PRE_REPORT_TIME:" + DateTime.Now + " PRE_STATUS_CHANGE_TIME:" + DateTime.Now +
                        " LOT_QTY1:" + LOT_QTY1 + " LOT_QTY2:" + LOT_QTY2 + " LOCATION:"+ LOCATION + " ROUTE_SID:" + ROUTE_SID + " OPER_FIRST_CHECK_IN_TIME:" + DateTime.Now +
                        " SHIFT_SID:"+ SHIFT_SID + " WORKGROUP_SID:" + WORKGROUP_SID + " LOT_SUB_STATUS_CODE:" + LOT_SUB_STATUS_CODE + " NEXT_OPERATION_SID:" + NEXT_OPERATION_SID +
                        " NEXT_OPERATION_CODE:"+ NEXT_OPERATION_CODE+ " NEXT_OPERATION_NAME:" + NEXT_OPERATION_NAME + " NEXT_OPERATION_SEQ:" + NEXT_OPERATION_SEQ +
                        " ')", true);
                */


                /*
                //ExecuteSqlTransaction https://msdn.microsoft.com/zh-tw/library/86773566(v=vs.110).aspx
                using (SqlConnection conn = new SqlConnection(conString))
                {
                    conn.Open();

                    SqlCommand command = conn.CreateCommand();
                    SqlTransaction transaction;

                    // Start a local transaction.
                    transaction = conn.BeginTransaction("SampleTransaction");

                    // Must assign both transaction object and connection
                    // to Command object for a pending local transaction
                    command.Connection = conn;
                    command.Transaction = transaction;

                    try
                    {
                        command.CommandText =
                            "Insert into Region (RegionID, RegionDescription) VALUES (100, 'Description')";
                        command.ExecuteNonQuery();
                        command.CommandText =
                            "Insert into Region (RegionID, RegionDescription) VALUES (101, 'Description')";
                        command.ExecuteNonQuery();

                        // Attempt to commit the transaction.
                        transaction.Commit();
                        Console.WriteLine("Both records are written to database.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                        Console.WriteLine("  Message: {0}", ex.Message);

                        // Attempt to roll back the transaction.
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            // This catch block will handle any errors that may have occurred
                            // on the server that would cause the rollback to fail, such as
                            // a closed connection.
                            Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                            Console.WriteLine("  Message: {0}", ex2.Message);
                        }
                    }
                }
                */


                //1	於WIP_LOT_HIST中INSERT資料
                //---1.1報工開始----
                sql = "INSERT INTO WIP_LOT_HIST " +
                "(WIP_LOT_HIST_SID, DATA_LINK_SID, LOT_SID, LOT, ALIAS_LOT1, ALIAS_LOT2, LOT_STATUS_SID, LOT_STATUS_CODE, " +
                "PRE_LOT_STATUS_SID, PRE_LOT_STATUS_CODE, WO_SID, WO, OPERATION_LINK_SID, OPERATION_SID, "+
                "OPERATION_CODE, OPERATION_NAME, OPERATION_SEQ, OPERATION_FINISH, PART_SID, PART_NO, "+
                "LOT_QTY, TOTAL_OK_QTY, TOTAL_NG_QTY, TOTAL_DEFECT_QTY, TOTAL_USER_COUNT, FACTORY_SID, FACTORY_CODE, FACTORY_NAME, ACTION_CODE, CONTROL_MODE, INPUT_FORM_NAME, CREATE_USER, " +
                "CREATE_TIME, REPORT_TIME, PRE_REPORT_TIME, PRE_STATUS_CHANGE_TIME, LOCATION, ROUTE_SID, SHIFT_SID, WORKGROUP_SID, " +
                "LOT_SUB_STATUS_CODE, NEXT_OPERATION_SID, NEXT_OPERATION_CODE, NEXT_OPERATION_NAME, NEXT_OPERATION_SEQ) " +
                "VALUES " +
                "(dbo.GetSid(), dbo.GetSid(), " + LOT_SID + ", " + LOT + ", '', '', '2', 'Run', "+ 
                " '1' , 'Wait', " + WO_SID + ", " + WO + ", "+OPERATION_LINK_SID+", "+ OPERATION_SID + ", '"+ 
                OPERATION_CODE + "', '"+ OPERATION_NAME + "', "+ OPERATION_SEQ + ", '"+ OPERATION_FINISH + "', "+ PART_SID + ", '" + PART_NO + "', " + 
                LOT_QTY + ", "+ OK_QTY + ", "+ NG_QTY + ", '0', '1', "+ FACTORY_SID + ", '"+ FACTORY_CODE + "', '"+ FACTORY_NAME + "', 'CHECK_IN', 'ONE', '"+ INPUT_FORM_NAME + "', '"+ USER + "',  " +
                " '" + Start + "', '" + Start + "' , '" + Start + "', '" + Start + "', '', " + ROUTE_SID + ", "+ SHIFT_SID + ", "+ WORKGROUP_SID + ", '" +
                LOT_SUB_STATUS_CODE + "', null, null, null, null)";


                command = new SqlCommand(sql, conn);
                command.Connection.Open();
                reader = command.ExecuteReader();
                command.Connection.Close();

                //取得剛剛寫入的那筆insert的WIP_LOT_HIST，並把他宣告給IN_WIP_LOT_HIST_SID
                sql = "select TOP (1) WIP_LOT_HIST_SID from WIP_LOT_HIST where INPUT_FORM_NAME = '"+ INPUT_FORM_NAME + "' AND LOT = " + LOT + " AND ACTION_CODE = 'CHECK_IN' order by WIP_LOT_HIST_SID desc";
                command = new SqlCommand(sql, conn);
                command.Connection.Open();
                reader = command.ExecuteReader();

                string IN_WIP_LOT_HIST_SID = null;
                while (reader.Read())
                {
                    IN_WIP_LOT_HIST_SID = reader[0].ToString();
                }
                command.Connection.Close();


                //---1.2報工結束----
                sql = "INSERT INTO WIP_LOT_HIST " +
                "(WIP_LOT_HIST_SID, DATA_LINK_SID, LOT_SID, LOT, ALIAS_LOT1, ALIAS_LOT2, LOT_STATUS_SID, LOT_STATUS_CODE, " +
                "PRE_LOT_STATUS_SID, PRE_LOT_STATUS_CODE, WO_SID, WO, OPERATION_LINK_SID, OPERATION_SID, " +
                "OPERATION_CODE, OPERATION_NAME, OPERATION_SEQ, OPERATION_FINISH, PART_SID, PART_NO, " +
                "LOT_QTY, TOTAL_OK_QTY, TOTAL_NG_QTY, TOTAL_DEFECT_QTY, TOTAL_USER_COUNT, FACTORY_SID, FACTORY_CODE, FACTORY_NAME, ACTION_CODE, CONTROL_MODE, INPUT_FORM_NAME, CREATE_USER, " +
                "CREATE_TIME, REPORT_TIME, PRE_REPORT_TIME, PRE_STATUS_CHANGE_TIME, LOCATION, ROUTE_SID, SHIFT_SID, WORKGROUP_SID, " +
                "LOT_SUB_STATUS_CODE, NEXT_OPERATION_SID, NEXT_OPERATION_CODE, NEXT_OPERATION_NAME, NEXT_OPERATION_SEQ) " +
                "VALUES " +
                "(dbo.GetSid(), dbo.GetSid(), " + LOT_SID + ", " + LOT + ", '', '', '2', 'Run', " +
                " '2' , 'Run', " + WO_SID + ", " + WO + ", " + OPERATION_LINK_SID + ", " + OPERATION_SID + ", '" +
                OPERATION_CODE + "', '" + OPERATION_NAME + "', " + OPERATION_SEQ + ", '" + OPERATION_FINISH + "', " + PART_SID + ", '" + PART_NO + "', " +
                LOT_QTY + ", " + OK_QTY + ", " + NG_QTY + ", '0', '1', " + FACTORY_SID + ", '" + FACTORY_CODE + "', '" + FACTORY_NAME + "', 'CHECK_OUT', 'ONE', '" + INPUT_FORM_NAME + "', '" + USER + "',  " +
                " '" + End + "', '" + End + "' , '" + End + "', '" + End + "', '', " + ROUTE_SID + ", " + SHIFT_SID + ", " + WORKGROUP_SID + ", '" +
                LOT_SUB_STATUS_CODE + "', null, null, null, null)";

                command = new SqlCommand(sql, conn);
                command.Connection.Open();
                reader = command.ExecuteReader();
                command.Connection.Close();

                //取得剛剛寫入的那筆insert的WIP_LOT_HIST，並把他宣告給OUT_WIP_LOT_HIST_SID
                sql = "select TOP (1) WIP_LOT_HIST_SID from WIP_LOT_HIST where INPUT_FORM_NAME = '" + INPUT_FORM_NAME + "' AND LOT = " + LOT + " AND ACTION_CODE = 'CHECK_OUT' order by WIP_LOT_HIST_SID desc";
                command = new SqlCommand(sql, conn);
                command.Connection.Open();
                reader = command.ExecuteReader();

                string OUT_WIP_LOT_HIST_SID = null;
                while (reader.Read())
                {
                    OUT_WIP_LOT_HIST_SID = reader[0].ToString();
                }
                command.Connection.Close();


                string ZZ_BEFER_PRINT_FINISH_QTY = null, ZZ_BEFER_PRINT_MATERIAL_QTY = null, FIN_CONF = null;
                string s = typeSelect.SelectedIndex.ToString();
                string Preprint = PreprintSec.SelectedValue.Trim();

                if (s.Equals("1"))
                {
                    if (Preprint == "PrePrintDone") {
                        ZZ_BEFER_PRINT_FINISH_QTY = PRINT_FIN_QTY;
                        ZZ_BEFER_PRINT_MATERIAL_QTY = "0";
                    }
                    else{
                        ZZ_BEFER_PRINT_FINISH_QTY = "0";
                        ZZ_BEFER_PRINT_MATERIAL_QTY = PRINT_FIN_QTY;
                    }

                }




                //2	於WIP_LOT_USER_HIST中INSERT資料

                /*
                sql = "INSERT INTO WIP_LOT_USER_HIST " +
"(WIP_LOT_USER_HIST_SID, IN_WIP_LOT_HIST_SID, OUT_WIP_LOT_HIST_SID, CREATE_USER, USER_COMMENT, " +
 "CREATE_IN_TIME, REPORT_IN_TIME, CREATE_OUT_TIME, REPORT_OUT_TIME, " +
 "OUT_FLAG, OUT_OK_QTY, OUT_NG_QTY, REPORT_OUT_OK_QTY, REPORT_OUT_NG_QTY, OPERATION_FINISH, OPERATION_LINK_SID, " +
 "SHIFT_SID, WORKGROUP_SID, LOT_SUB_STATUS_CODE, OUT_USER, OUT_SHIFT_SID, OUT_WORKGROUP_SID, " +
 "ZZ_SUB_USER_COUNT, ZZ_CHANGE_SHIFT_FLAG, ZZ_PLATE_FLAG, ZZ_BEFER_PRINT_FINISH_QTY, ZZ_BEFER_PRINT_MATERIAL_QTY, FIN_CONF " +
 //", EDIT_USER, EDIT_TIME"+
 ") " +
"VALUES " +
"(dbo.GetSid(), "+ IN_WIP_LOT_HIST_SID + ", "+ OUT_WIP_LOT_HIST_SID + ", '"+ USER + "', '', " +
 " '" + Start + "', '" + Start + "', '" + End + "', '" + End + "', " +
 " 'Y', "+ OK_QTY + ", "+ NG_QTY + ", "+ OK_QTY + ", "+ NG_QTY + ", 'N', "+ OPERATION_LINK_SID + ", " +
SHIFT_SID + ", "+ WORKGROUP_SID + ", 'Prepare', '"+ USER + "', "+ SHIFT_SID + ", "+ WORKGROUP_SID + ", " +
 " '0', 'N', 'N', "+ ZZ_BEFER_PRINT_FINISH_QTY + ", "+ ZZ_BEFER_PRINT_MATERIAL_QTY + ", " + FIN_CONF + 
// ", '"+ USER + "', 'GETDATE()' "+
 " )";

            */
                sql = "INSERT INTO WIP_LOT_USER_HIST " +
"(WIP_LOT_USER_HIST_SID, IN_WIP_LOT_HIST_SID, OUT_WIP_LOT_HIST_SID, CREATE_USER, USER_COMMENT, "+
 "CREATE_IN_TIME, REPORT_IN_TIME, CREATE_OUT_TIME, REPORT_OUT_TIME " +
" ) " +
"VALUES " +
"(dbo.GetSid(), " + IN_WIP_LOT_HIST_SID + ", " + OUT_WIP_LOT_HIST_SID + ", '" + USER + "' , '"+ INPUT_FORM_NAME + "' ,"+
 " '" + Start + "', '" + Start + "', '" + End + "', '" + End + "' " +
" )";


                command = new SqlCommand(sql, conn);
                command.Connection.Open();
                reader = command.ExecuteReader();
                command.Connection.Close();

                //取得剛剛寫入的那筆insert的WIP_LOT_USER_HIST_SID，並把他宣告出來
                sql = "select TOP (1) WIP_LOT_USER_HIST_SID from WIP_LOT_USER_HIST where IN_WIP_LOT_HIST_SID = "+ IN_WIP_LOT_HIST_SID + " and  OUT_WIP_LOT_HIST_SID = "+ OUT_WIP_LOT_HIST_SID + " and CREATE_USER = '"+ USER + "' order by CREATE_IN_TIME desc";
                command = new SqlCommand(sql, conn);
                command.Connection.Open();
                reader = command.ExecuteReader();

                string WIP_LOT_USER_HIST_SID = null;
                while (reader.Read())
                {
                    WIP_LOT_USER_HIST_SID = reader[0].ToString();
                }
                command.Connection.Close();




                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('IN: "+ IN_WIP_LOT_HIST_SID + " OUT: "+ OUT_WIP_LOT_HIST_SID + "  WIP_LOT_USER_HIST_SID: "+ WIP_LOT_USER_HIST_SID + " ')", true);


                /*
                //---查工序----------------
                //先偵測是否用的是子批(有.001、.002的)
                if (WO.Contains('.'))
                {
                    WO = WO.Substring(0, WO.IndexOf('.')); //如果工單號碼是11030018821.001會被改成11030018821   
                }
                sql = "select SAP_SEQ from V_BAS_ROUTE_OPER where  ROUTE_CODE= '" + WO + "' order by SAP_SEQ desc";
                command = new SqlCommand(sql, conn);
                command.Connection.Open();
                reader = command.ExecuteReader();
                */

                /*
                }
                catch (SqlException excep)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('出現錯誤!')", true);
                    System.Diagnostics.Debug.WriteLine("==========Exception=========");
                    System.Diagnostics.Debug.WriteLine("myDBInit()執行時，出現錯誤:" + excep);
                }
                */

            }
            else if (STATUS == "Production")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('走加工!')", true);
            }
            else if (STATUS == "Clean")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('走清理!')", true);
            }
            else if (STATUS == "Init")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('走分批!')", true);
            }
            else if (STATUS == "Exception")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('走除外!')", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('發生異常狀態!')", true);
            }





        }

    }
}