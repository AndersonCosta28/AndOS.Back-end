using AndOS.Application.Folders.Get.GetAccountFolderInParentFolder;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Files.Get.GetById;

namespace AndOS.Application.Files.Get.GetById
{
    public class GetFileByIdHandler(IRepository<File> fileReadRepository, ICloudStorageServiceFactory cloudStorageServiceFactory, IStringLocalizer<ValidationResource> validationLocalizer, IMapperService mapperService, ISender sender) : IRequestHandler<GetFileByIdRequest, GetFileByIdResponse>
    {
        public async Task<GetFileByIdResponse> Handle(GetFileByIdRequest request, CancellationToken cancellationToken)
        {
            File file = await fileReadRepository.FirstOrDefaultAsync(new GetFileByIdSpec(request.Id), cancellationToken) ??
                            throw new ApplicationLayerException(validationLocalizer["FileNotFound"]);

            var accountFolder = await sender.Send(new GetAccountFolderInParentFolderRequest(file.ParentFolder.Id), cancellationToken);

            var account = accountFolder.Account;

            ICloudStorageService cloudStorageService = cloudStorageServiceFactory.GetCloudStorageService(account.CloudStorage);
            string url = await cloudStorageService.GetUrlDownloadFileAsync(file, account);
            var result = await mapperService.MapAsync<GetFileByIdResponse>(file);
            result.Url = url;
            result.CloudStorage = account.CloudStorage;
            return result;
        }
    }
}
