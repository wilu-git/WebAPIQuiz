using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace WebAPIQuiz
{
    public class Requirement
    {
//        Requirements
//Authentication Flow
//Login Endpoint(POST /api/auth/login)
//Conditions
//Requires X-API-KEY in header
//Validate API key + expiration
//Returns
//JWT Token
//Accepts JSON
//        {
//     "username": "admin",
//     "password": "1234"
//}
//        Rotating API Keys stored in appsettings.JSON
//        If expired → return 401 Unauthorized with a custom message
//        Use at least 2 roles
//        Admin
//User
//Rules
//Admin -> Full access
//User
//GET Task
//POST Task
//Task Model(In-Memory)
//class TaskItem
//        {
//            public int Id { get; set; }
//            public string Title { get; set; }
//            public bool IsCompleted { get; set; }
//        }
//        Store it as a static list
//        RESTFul Endpoints
//        Method  Endpoint Access
//GET	/api/tasks User, Admin
//GET	/api/tasks/{id
//    }
//    User, Admin
//POST	/api/tasks User, Admin
//PUT	/api/tasks/{id
//}
//Admin only
//DELETE	/api/tasks/{id}	Admin only
//Rate Limiting
//Login -> 3 request per minute
//Tasks -> 10 request per minute
//Custom Attribute for custom Header
//Behavior
//Reads X-API-KEY from header
//Validates Expiration
//Only applied to /login
//JWT Requirements
//Include
//Username
//Role
//Expiration -> 5minutes
//Bonus
//Refresh Token
//POST /api/auth/refresh
//Condition
//The Login endpoint would then need to return a refresh token
//Returns
//New JWT
//New Refresh Token
//Accepts: JSON
//{
//      "refreshToken": "string"
//}
//Logout Endpoint -> Invalidate Refresh Token
//Separate rate limit per role
//Logs user logins with the following
//Role
//Username
//JWT with Refresh Token if implemented
//PLEASE SUBMIT YOUR GITHUB LINK CORRECTLY!!
    }
}
