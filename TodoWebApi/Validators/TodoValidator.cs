using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TodoWebApi.Models.Auth;
using TodoWebApi.Models.ToDo;

namespace TodoWebApi.Validators
{
    public static class TodoValidator
    {
        public static string AddTodoValidate(AddTodoRequestModel model)
        {
            if (model == null)
                return "Request data is missing";
            if (model.UserId <= 0)
                return "UserId is required";

            if (string.IsNullOrWhiteSpace(model.Title))
                return "Title is required";

            if (model.Title.Length > 100)
                return "Todo title can not exceed 100 charachters";

            if (string.IsNullOrWhiteSpace(model.Description))
                return "Todo description is required";

            if (model.DueDate < DateTime.Today)
                return "Duedate can not be in the past";

            return null;
        }

        public static string GetTodoValidate(GetTodoRequestModel model)
        {
            if (model == null)
                return "Request data is missing";
            if (model.UserId <= 0)
                return "UserId is required";
            return null;
        }

        public static string DeleteTodoValidate(DeleteTodoRequestModel model)
        {
            if (model == null)
                return "Request data is missing";
            if (model.UserId <= 0)
                return "UserId is required";
            return null;
        }

        public static string UpdateTodoValidate(UpdateTodoRequestModel model)
        {
            if (model == null)
                return "Request data is missing";
            if (model.UserId <= 0)
                return "UserId is required";
            if (model.TodoId <= 0)
                return "Todo Id is required";

            if (string.IsNullOrWhiteSpace(model.Title))
                return "Title is required";

            if (model.Title.Length > 100)
                return "Todo title can not exceed 100 charachters";

            if (string.IsNullOrWhiteSpace(model.Description))
                return "Todo description is required";

            if (model.DueDate < DateTime.Today)
                return "Duedate can not be in the past";

            return null;
        }
    }
}