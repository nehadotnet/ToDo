using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TodoWebApi.Data;
using TodoWebApi.Models.Common;
using TodoWebApi.Models.ToDo;

namespace TodoWebApi.Controllers
{
    [RoutePrefix("api/todo")]
    public class ToDoController : ApiController
    {
        [Route("add_todo")]
        [HttpPost]
        public HttpResponseMessage AddTodo([FromBody] AddTodoRequestModel addTodoRequestModel)
        {
            ApiResponse<AddTodoResponseModel> apiResponse = new ApiResponse<AddTodoResponseModel>();
            try
            {
                using (SqlConnection con = DbHelper.GetConnection())
                using (SqlCommand cmd = new SqlCommand("Spr_ToDo_CRUD", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ToDo_Id", 0);
                    cmd.Parameters.AddWithValue("@UserId", addTodoRequestModel.UserId);
                    cmd.Parameters.AddWithValue("@ToDo_Title", addTodoRequestModel.Title);
                    cmd.Parameters.AddWithValue("@Description", addTodoRequestModel.Description);
                    cmd.Parameters.AddWithValue("@Due_date", addTodoRequestModel.DueDate);
                    cmd.Parameters.AddWithValue("@Action", "C");

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            apiResponse.Status = 500;
                            apiResponse.ErrorMsg = "Unexpected Error";
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
                        }

                        int status = Convert.ToInt32(reader["Status"]);

                        apiResponse.Status = status;
                        apiResponse.Message = reader["Message"].ToString();
                        if (status == 200)
                        {
                            apiResponse.Data = new AddTodoResponseModel
                            {
                                ToDo_Id = Convert.ToInt32(reader["ToDo_Id"])
                            };
                        }
                        return Request.CreateResponse((HttpStatusCode)status, apiResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                apiResponse.Status = 500;
                apiResponse.ErrorMsg = ex.Message;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [Route("get_todo")]
        [HttpPost]
        public HttpResponseMessage TodoList([FromBody] ReadTodoRequestModel readTodoRequestModel)
        {
            ApiResponse<List<ReadTodoResponseModel>> apiResponse = new ApiResponse<List<ReadTodoResponseModel>>();

            try
            {
                using (SqlConnection con = DbHelper.GetConnection())
                using (SqlCommand cmd = new SqlCommand("Spr_ToDo_CRUD", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ToDo_Id", 0);
                    cmd.Parameters.AddWithValue("@UserId", readTodoRequestModel.UserId);
                    cmd.Parameters.AddWithValue("@ToDo_Title", "");
                    cmd.Parameters.AddWithValue("@Description", "");
                    cmd.Parameters.AddWithValue("@Due_date", "");
                    cmd.Parameters.AddWithValue("@Action", "R");

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<ReadTodoResponseModel> todos = new List<ReadTodoResponseModel>();
                        while (reader.Read())
                        {
                            todos.Add(new ReadTodoResponseModel
                            {
                                TodoId = Convert.ToInt32(reader["ToDo_Id"]),
                                Title = reader["ToDo_Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                DueDate =reader["Due_date"].ToString(),
                                RemainingDays = Convert.ToInt32(reader["Remaining_Days"])
                            });
                        }

                        apiResponse.Status = 200;
                        apiResponse.Data = todos;
                        apiResponse.Message = "Todo list fetched successfully";

                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);


                    }
                }
            }
            catch (Exception ex)
            {
                apiResponse.Status = 500;
                apiResponse.ErrorMsg = ex.Message;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

    }
}
