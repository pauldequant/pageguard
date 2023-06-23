using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace PageGuard.Controllers
{
    using Microsoft.Extensions.Logging;
    using NPoco;
    using Models;
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Umbraco.Cms.Core.Mail;
    using Umbraco.Cms.Core.Models.Membership;
    using Umbraco.Cms.Infrastructure.Scoping;
    using Umbraco.Cms.Web.BackOffice.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Schemas;
    using Umbraco.Cms.Core.Models.Email;
    using Umbraco.Cms.Core.Services;
    using Umbraco.Cms.Web.Common.Attributes;
    using Umbraco.Cms.Core.Web;
    using Umbraco.Cms.Core.Routing;
    using Umbraco.Cms.Web.Common.UmbracoContext;

    [PluginController("PageGuardApi")]
    [IsBackOffice]
    public class PageGuardApiController : UmbracoAuthorizedJsonController
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<PageGuardApiController> _logger;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly ILocalizationService _localizationService;

        public PageGuardApiController(IScopeProvider scopeProvider, ILogger<PageGuardApiController> logger, IUserService userService, IEmailSender emailSender, IUmbracoContextAccessor umbracoContextAccessor, ILocalizationService localizationService)
        {
            _scopeProvider = scopeProvider;
            _userService = userService;
            _emailSender = emailSender;
            _logger = logger;
            _umbracoContextAccessor = umbracoContextAccessor;
            _localizationService = localizationService;
        }

        [HttpPost]
        public bool CheckOutPage(int nodeId, int userId)
        {
            this.PageGuardOperation(nodeId, userId, (int)PageGuardStatusModel.PageStatus.CheckedOut);

            return true;
        }

        [HttpPost]
        public bool CheckInPage(int nodeId, int userId)
        {
            this.PageGuardOperation(nodeId, userId, (int)PageGuardStatusModel.PageStatus.CheckedIn);

            return true;
        }

        [HttpPost]
        public bool OverridePage(int nodeId, int userId)
        {
            this.PageGuardOperation(nodeId, userId, (int)PageGuardStatusModel.PageStatus.Override);

            return true;
        }

        [HttpPost]
        public async Task<bool> Notify(int nodeId, int userId, int myUserId)
        {
            try
            {
                IUser userAccount = _userService.GetUserById(userId);

                string ownersName = userAccount?.Name;
                string ownersEmail = userAccount?.Email;

                userAccount = _userService.GetUserById(myUserId);

                if (userAccount != null)
                {
                    string sender = _localizationService.GetDictionaryItemByKey("PageGuardDefaultSender")?.ToString();
                    string recipient = ownersEmail;
                    string subject = _localizationService.GetDictionaryItemByKey("PageGuardNotifySubject")?.ToString();
                    
                    StringBuilder stringBuilder = new StringBuilder();

                    _umbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext umbracoContext);
                    string url = umbracoContext?.Content?.GetById(nodeId)?.Url(null,UrlMode.Absolute);
                   
                    stringBuilder.AppendFormat("Hello {0},", ownersName);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("You currently have the following page checked out: ");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendFormat("{0},", url);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendFormat("The user '{0}' has requested access to this page.", userAccount.Name);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("If you are finished with this page, please login to the Umbraco CMS and check in the page so this user can access it.");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("Thanks");

                    string body = stringBuilder.ToString();

                    EmailMessage message = new EmailMessage(sender, recipient, subject, body, true);
                    await _emailSender.SendAsync(message, emailType: "Notify");

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Warning, ex, "PageGuard: Could not send a notification, please check SMTP settings");
            }

            return false;
        }

        [HttpGet]
        public bool CheckPageStatus(int nodeId, int userId)
        {
            using var scope = _scopeProvider.CreateScope();

            bool showPrompt = true;

            var sqlQuery = new Sql().Select("Id, NodeId, UserId").From("PageGuardStatus").Where("[NodeId] = @0", nodeId);

            List<PageStatusQueryModel> sqlResults = scope.Database.Fetch<PageStatusQueryModel>(sqlQuery);

            scope.Complete();

            if (sqlResults.Count > 0)
            {
                // Page Checked Out - By Me
                showPrompt = !sqlResults.Any(x => x.UserId == userId);
                // Page Checked Out - By Someone else (Show Already Checked Out Box)
            }

            return showPrompt;
        }

        [HttpGet]
        public List<LatestPageStatusModel> CheckCurrentAccessDetails(int nodeId)
        {
            using var scope = _scopeProvider.CreateScope();

            var sqlQuery = new Sql().Select("[P].[Id], [P].[NodeId], [P].[UserId], [P].[DateTimeRecord], [U].[userName], [U].[userEmail], [U].[avatar]")
                                    .From("PageGuardStatus P")
                                    .LeftJoin("umbracoUser U")
                                    .On("[P].[UserId] = [U].[id]")
                                    .Where("[NodeId] = @0", nodeId);

            List<LatestPageStatusModel> sqlResults = scope.Database.Fetch<LatestPageStatusModel>(sqlQuery);

            scope.Complete();

            return sqlResults;
        }

        [HttpGet]
        public List<AdminListForNotificationsModel> LoadAdminListForNotifications()
        {
            using var scope = _scopeProvider.CreateScope();

            int adminGroupId = 1;
            var sqlQuery = new Sql().Select("id, userName, userEmail, avatar").From("umbracoUser U").InnerJoin("umbracoUser2UserGroup UG").On("[U].[id] = [UG].[userId]").Where("[UG].[userGroupId] = @0", adminGroupId);

            List<AdminListForNotificationsModel> sqlResults = scope.Database.Fetch<AdminListForNotificationsModel>(sqlQuery);

            scope.Complete();

            return sqlResults;
        }

        [HttpGet]
        public List<MyCheckOutsModel> LoadAllMyCheckOuts(int userId)
        {
            using var scope = _scopeProvider.CreateScope();

            var sqlQuery = new Sql().Select("[U].[text], [P].[DateTimeRecord], [P].[NodeId]").From("PageGuardStatus P").InnerJoin("umbracoNode U").On("[P].NodeId = [U].id").Where("[P].[UserId] = @0", userId);

            List<MyCheckOutsModel> sqlResults = scope.Database.Fetch<MyCheckOutsModel>(sqlQuery);

            scope.Complete();

            return sqlResults;
        }

        [HttpGet]
        public List<AllCheckoutsModel> LoadEveryonesCheckouts(int userId)
        {
            using var scope = _scopeProvider.CreateScope();

            var sqlQuery = new Sql().Select("[P].[Id], [P].[NodeId], [P].[UserId], [P].[DateTimeRecord], [U].[userName], [U].[userEmail], [u].[avatar], [N].[text]")
                .From("PageGuardStatus P")
                .LeftJoin("umbracoUser U")
                .On("[P].[UserId] = [U].[id]")
                .LeftJoin("umbracoNode N")
                .On("[P].NodeId = [N].id")
                .Where("[UserId] <> @0", userId);

            var sqlResults = scope.Database.Fetch<AllCheckoutsModel>(sqlQuery);
            
            scope.Complete();

            return sqlResults.ToList();
        }

        [HttpGet]
        public string GetCurrentUserEmail(int userId)
        {
            using var scope = _scopeProvider.CreateScope();

            string userEmailAddress = string.Empty;

            var sqlQuery = new Sql().Select("userEmail").From("umbracoUser").Where("[id] = @0", userId);

            var sqlResults = scope.Database.Fetch<CurrentUserEmailModel>(sqlQuery);

            if (sqlResults != null && sqlResults.Count > 0)
            {
                userEmailAddress = sqlResults.Select(s => s.Email).FirstOrDefault();
            }
            else
            {
                userEmailAddress = "Email Unknown";
            }

            scope.Complete();

            return userEmailAddress;
        }

        [HttpGet]
        public List<LatestPageStatusModel> GetLastFiveActions(int nodeId)
        {
            using var scope = _scopeProvider.CreateScope();

            var sqlQuery = new Sql().Select("[P].[Id], [P].[NodeId], [P].[UserId], [P].[DateTimeRecord], [P].[Status], [U].[userName], [U].[userEmail], [u].[avatar]").From("PageGuardHistory P").LeftJoin("umbracoUser U").On("[P].[UserId] = [U].[id]").Where("[NodeId] = @0", nodeId).OrderBy("DateTimeRecord DESC");

            var sqlResults = scope.Database.Fetch<LatestPageStatusModel>(sqlQuery).Take(5);

            scope.Complete();

            return sqlResults.ToList();
        }

        [HttpGet]
        public int GetLatestPageStatus(int nodeId)
        {
            using var scope = _scopeProvider.CreateScope();

            int currentPageStatus = 0;

            var sqlQuery = new Sql().Select("[P].[Id], [P].[NodeId], [P].[UserId], [P].[DateTimeRecord], [P].[Status]").From("PageGuardHistory P").Where("[NodeId] = @0", nodeId).OrderBy("DateTimeRecord DESC");

            var sqlResults = scope.Database.Fetch<LatestPageStatusModel>(sqlQuery).Take(1);

            scope.Complete();

            if (sqlResults != null)
            {
                currentPageStatus = sqlResults.Select(s => s.Status).SingleOrDefault();
            }

            return currentPageStatus;
        }

        [HttpGet]
        public string GetPageStatusLabel(int status)
        {
            string statusLabel = string.Empty;

            switch (status)
            {
                case 1:
                    statusLabel = "Checked In";
                    break;
                case 2:
                    statusLabel = "Checked Out";
                    break;
                case 3:
                    statusLabel = "Checked Out";
                    break;
                default:
                    statusLabel = "Unknown";
                    break;
            }

            return statusLabel;
        }

        public void PageGuardOperation(int nodeId, int userId, int status)
        {
            bool safeToArchive = true;
            DateTime actionDateTime = DateTime.UtcNow;

            using var scope = _scopeProvider.CreateScope();

            try
            {
                if (status == (int)PageGuardStatusModel.PageStatus.CheckedOut)
                {
                    PageGuardStatusSchema pageGuardStatus = new PageGuardStatusSchema
                    {
                        DateTimeRecord = actionDateTime,
                        NodeId = nodeId,
                        UserId = userId
                    };

                    scope.Database.Insert(pageGuardStatus);
                }

                if (status == (int)PageGuardStatusModel.PageStatus.CheckedIn || status == (int)PageGuardStatusModel.PageStatus.Override)
                {
                    var sqlQuery = new Sql().Select("*").From("PageGuardStatus").Where("[NodeId] = @0", nodeId);
                    var sqlResults = scope.Database.Fetch<PageGuardStatusSchema>(sqlQuery).FirstOrDefault();

                    if (sqlResults != null)
                    {
                        var deletedNodeId = scope.Database.Delete(sqlResults);
                    }
                }
            }
            catch (Exception ex)
            {
                safeToArchive = false;
                _logger.Log(LogLevel.Warning, ex, "PageGuard: Could not set the page lock status");
            }

            if (safeToArchive)
            {
                PageGuardHistorySchema pageGuardHistory = new PageGuardHistorySchema()
                {
                    NodeId = nodeId,
                    UserId = userId,
                    DateTimeRecord = actionDateTime,
                    Status = status
                };

                scope.Database.Insert(pageGuardHistory);

            }

            scope.Complete();
        }

        public bool ShowCheckInMenuItem(int nodeId)
        {
            using var scope = _scopeProvider.CreateScope();

            var sqlQuery = new Sql().Select("Id, NodeId, UserId").From("PageGuardStatus").Where("[NodeId] = @0", nodeId);

            List<PageStatusQueryModel> sqlResults = scope.Database.Fetch<PageStatusQueryModel>(sqlQuery);

            scope.Complete();

            if (sqlResults != null && sqlResults.Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}
