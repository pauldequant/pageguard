using Microsoft.Extensions.DependencyInjection;
using PageGuard.Component;

namespace PageGuard.Composer
{
    using Handlers;
    using Umbraco.Cms.Core.Composing;
    using Umbraco.Cms.Core.DependencyInjection;
    using Umbraco.Cms.Core.Notifications;

    public class PageGuardComposer : ComponentComposer<PageGuardComponent>
    {
        public override void Compose(IUmbracoBuilder builder)
        {
            base.Compose(builder);

            builder.AddNotificationHandler<ContentPublishedNotification, PageGuardPublishedNotification>();
            builder.AddNotificationHandler<ContentSentToPublishNotification, PageGuardSentToPublishNotification>();
            builder.AddNotificationHandler<ContentUnpublishedNotification, PageGuardUnPublishedNotification>();
            builder.AddNotificationHandler<MenuRenderingNotification, PageGuardMenuRenderingNotification>();
            builder.AddNotificationHandler<TreeNodesRenderingNotification, PageGuardTreeNodesRenderingNotification>();
        }
    }
}