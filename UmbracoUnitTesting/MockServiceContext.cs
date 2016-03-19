using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Services;

namespace UmbracoUnitTesting
{
    public class MockServiceContext
    {
        public readonly ServiceContext ServiceContext;
        public Mock<IApplicationTreeService> ApplicationTreeService = new Mock<IApplicationTreeService>();
        public Mock<IAuditService> AuditService = new Mock<IAuditService>();
        public Mock<IContentService> ContentService = new Mock<IContentService>();
        public Mock<IContentTypeService> ContentTypeService = new Mock<IContentTypeService>();
        public Mock<IDataTypeService> DataTypeService = new Mock<IDataTypeService>();
        public Mock<IDomainService> DomainService = new Mock<IDomainService>();
        public Mock<IEntityService> EntityService = new Mock<IEntityService>();
        public Mock<IExternalLoginService> ExternalLoginService = new Mock<IExternalLoginService>();
        public Mock<IFileService> FileService = new Mock<IFileService>();
        public Mock<ILocalizationService> LocalizationService = new Mock<ILocalizationService>();
        public Mock<IMacroService> MacroService = new Mock<IMacroService>();
        public Mock<IMediaService> MediaService = new Mock<IMediaService>();
        public Mock<IMemberGroupService> MemberGroupService = new Mock<IMemberGroupService>();
        public Mock<IMemberService> MemberService = new Mock<IMemberService>();
        public Mock<IMemberTypeService> MemberTypeService = new Mock<IMemberTypeService>();
        public Mock<IMigrationEntryService> MigrationEntryService = new Mock<IMigrationEntryService>();
        public Mock<INotificationService> NotificationService = new Mock<INotificationService>();
        public Mock<IPackagingService> PackagingService = new Mock<IPackagingService>();
        public Mock<IPublicAccessService> PublicAccessService = new Mock<IPublicAccessService>();
        public Mock<IRelationService> RelationService = new Mock<IRelationService>();
        public Mock<ISectionService> SectionService = new Mock<ISectionService>();
        public Mock<IServerRegistrationService> ServerRegistrationService = new Mock<IServerRegistrationService>();
        public Mock<ITagService> TagService = new Mock<ITagService>();
        public Mock<ITaskService> TaskService = new Mock<ITaskService>();
        public Mock<ILocalizedTextService> TextService = new Mock<ILocalizedTextService>();
        public Mock<IUserService> UserService = new Mock<IUserService>();
        public void Reset()
        {
            foreach (var service in new Mock[] {ContentService, MediaService, ContentTypeService, DataTypeService, FileService, LocalizationService,
                PackagingService, EntityService, RelationService, MemberGroupService, MemberTypeService, MemberService,
                UserService, SectionService, ApplicationTreeService, TagService, NotificationService, TextService, AuditService, DomainService,
                TaskService, MacroService, PublicAccessService, ExternalLoginService,
                MigrationEntryService})
            {
                service.Reset();
            }
        }

        public MockServiceContext()
        {
            this.ServiceContext = new ServiceContext(ContentService.Object, MediaService.Object, ContentTypeService.Object, DataTypeService.Object, FileService.Object, LocalizationService.Object,
                PackagingService.Object, EntityService.Object, RelationService.Object, MemberGroupService.Object, MemberTypeService.Object, MemberService.Object,
                UserService.Object, SectionService.Object, ApplicationTreeService.Object, TagService.Object, NotificationService.Object, TextService.Object, AuditService.Object, DomainService.Object,
                TaskService.Object, MacroService.Object, PublicAccessService.Object, ExternalLoginService.Object,
                MigrationEntryService.Object);
        } //IContentService contentService = null, IMediaService mediaService = null, IContentTypeService contentTypeService = null, IDataTypeService dataTypeService = null, IFileService fileService = null, ILocalizationService localizationService = null, IPackagingService packagingService = null, IEntityService entityService = null, IRelationService relationService = null, IMemberGroupService memberGroupService = null, IMemberTypeService memberTypeService = null, IMemberService memberService = null, IUserService userService = null, ISectionService sectionService = null, IApplicationTreeService treeService = null, ITagService tagService = null, INotificationService notificationService = null, ILocalizedTextService localizedTextService = null, IAuditService auditService = null, IDomainService domainService = null, ITaskService taskService = null, IMacroService macroService = null, IPublicAccessService publicAccessService = null, IExternalLoginService externalLoginService = null, IMigrationEntryService migrationEntryService = null);

    }
}
