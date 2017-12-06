using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ServiceStack.Common.Extensions;
using System.Web.Mvc;
using RedisClientSample.Models;

namespace RedisClientSample.Controllers
{
    public class QuestionController : Controller
    {
        public IRepository RedisRepo { get; set; }
        //
        // GET: /Question/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// http://localhost:37447/Question/GetQuestions?tag=test
        /// </summary>
        /// <param name="page"></param>
        /// <param name="tag"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetQuestions(int? page, string tag, int? userId)
        {
            // if data was not initialized, then add it
            if (RedisRepo.GetAllQuestions().Count == 0)
            {
                InitializeDataInCache();
                // Lets say it takes some time to load questions
                Thread.Sleep(4000);
            }

            List<QuestionResult> results = null;
            if (!tag.IsNullOrEmpty())
            {
                results = RedisRepo.GetQuestionsTaggedWith(tag);
                return ReturnQuestionResults(results);
            }

            if (userId.HasValue)
            {
                results = RedisRepo.GetQuestionsByUser(userId.Value);
                return ReturnQuestionResults(results);
            }

            var pageOffset = page.GetValueOrDefault(0) * 10;
            results = RedisRepo.GetRecentQuestionResults(pageOffset, pageOffset + 10);

            return ReturnQuestionResults(results);
        }

        private ActionResult ReturnQuestionResults(List<QuestionResult> results)
        {
            StringBuilder resultMessage = new StringBuilder("Total results: " + results.Count);
            foreach (QuestionResult questionResult in results)
            {
                resultMessage.AppendLine(" Quest Title: " + questionResult.Question.Title + " Content: " + questionResult.Question.Content + ";             ");
            }

            ViewBag.Message = resultMessage.ToString();
            return View("Index");
        }

        private void InitializeDataInCache()
        {
            RedisRepo.StoreQuestion(new Question
                {
                    Content = "first simple question",
                    CreatedDate = DateTime.Now,
                    Id = 123,
                    Tags = new List<string>{ "test", "test1", "test2", "test3" },
                    Title = "First question",
                    UserId = 5345345
                });
            
            RedisRepo.StoreQuestion(new Question
            {
                Content = "second simple question",
                CreatedDate = DateTime.Now,
                Id = 9895453,
                Tags = new List<string> { "test", "test1", "test2", "test3" },
                Title = "Second question",
                UserId = 12375643
            });

            RedisRepo.StoreQuestion(new Question
            {
                Content = "third simple question",
                CreatedDate = DateTime.Now,
                Id = 98467,
                Tags = new List<string> { "test", "test11", "test2", "test3" },
                Title = "Third question",
                UserId = 98954358
            });

            RedisRepo.StoreQuestion(new Question
            {
                Content = "forth simple question",
                CreatedDate = DateTime.Now,
                Id = 54345,
                Tags = new List<string> { "test453", "test1", "test21", "test3" },
                Title = "Forth question",
                UserId = 989356
            });

            RedisRepo.StoreQuestion(new Question
            {
                Content = "fifth simple question",
                CreatedDate = DateTime.Now,
                Id = 4534,
                Tags = new List<string> { "test543", "test1", "test2", "test33" },
                Title = "Fifth question",
                UserId = 45345234
            });
        }
    }
}
