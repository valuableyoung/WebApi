using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class DatabaseController : ControllerBase 
    {
        public string ScanCode { get; set; }
        public string SetStatus { get; set; }
        public DateTime Date { get; set; }

        public string SelectTestQuery = @"Select 
                                                  SKU.SKUname, 
                                                  SKU.SKUDescription,
                                                  SKU.Status,
                                                  SKU.Scancode
                                                  
                                           From 
                                                   [SKU_DB].[dbo].[SKUView] as Sku
                                           Where 
                                                   SKU.ScanCode  = @ScanCode;";

        public string SelectStatusesQuery = @"Select 
                                                       SKUStatuses.id,
                                                       SKUStatuses.status
                                                       
                                             From 
                                                       [SKU_DB].[dbo].[SKUStatuses]
                                             order by  
                                                       SKUStatuses.id asc ;";


        public string UpdateStatusQuery = @"Update SKU
                                                     set 
                                                            status_id = @SetStatus,
                                                            Change_date = @Date 
                                                     where  
                                                            scancode = @Scancode;";
        

        [HttpGet("getdata/{scancode}")]

        
        public DatabaseInfo GetData(string scancode)
        {
            DatabaseInfo info = new DatabaseInfo();
            ScanCode = scancode;
            var con = new SqlConnection(Config.connectiontring);
            var cmd = new SqlCommand(SelectTestQuery);
            cmd.Parameters.Add("@ScanCode", SqlDbType.NVarChar).Value = ScanCode;

            cmd.Connection = con;

            try
            {
                con.Open();
                info.ConnectionInfo = "Подключение к серверу: Успешно";
                SqlDataReader readerSKU = cmd.ExecuteReader();

                if (readerSKU.HasRows)
                {
                    while (readerSKU.Read())
                    {
                        
                        info.SKUName = "Наименование: " + readerSKU.GetString(0)+"\n";
                        info.SKUDescription = "Описание: " + readerSKU.GetString(1) + "\n";
                        info.Status = "Состояние: " + readerSKU.GetString(2) + "\n";  
                        info.Scancode = readerSKU.GetString(3);
                        
                    }

                    readerSKU.Close();
                    cmd = new SqlCommand(SelectStatusesQuery);
                    cmd.Connection = con;
                    SqlDataReader readerStatus = cmd.ExecuteReader();
                    
                    info.ChooseStatus = "Выберите статус:\n";

                    if (readerStatus.HasRows)
                    {
                        List<string>list = new List<string>();
                        while (readerStatus.Read())
                        {

                            list.Add(readerStatus.GetInt32(0).ToString() + " " + readerStatus.GetString(1)) ;//readerStatus.GetString(1)+ " " + readerStatus.GetInt32(0).ToString()
                            
                        }
                        readerStatus.NextResult();
                        info.statuses = list;
                    }
                   
                    return info;
                    //CheckData();

                }
                else
                {
                    //NextSession(2);
                    return info;
                }
            }
            catch
            {

                //Console.WriteLine("Подключение к серверу: Ошибка");
                throw;
            }

        }

        [HttpPost("setdata")]
        public void SetData([FromBody]string[] s)
        {
            var con = new SqlConnection(Config.connectiontring);
            var cmd = new SqlCommand(UpdateStatusQuery);
            Date = DateTime.Now;
            cmd.Parameters.Add("@ScanCode", SqlDbType.NVarChar).Value = s[0];
            cmd.Parameters.Add("@SetStatus", SqlDbType.Int).Value = s[1];
            cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Now;

            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();

            //NextSession(0);

        }


    }
}

