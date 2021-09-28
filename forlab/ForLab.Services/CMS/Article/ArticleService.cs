using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.CMS.Article;
using ForLab.Repositories.CMS.Article;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using ForLab.Core.Common;
using ForLab.Services.Global.FileService;
using ForLab.Repositories.CMS.ArticleImage;
using Microsoft.AspNetCore.Http;
using ForLab.Services.Global.UploadFiles;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace ForLab.Services.CMS.Article
{
    public class ArticleService : GService<ArticleDto, Data.DbModels.CMSSchema.Article, IArticleRepository>, IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IArticleImageRepository _articleImageRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IUploadFilesService _uploadFilesService;

        public ArticleService(IMapper mapper,
            IResponseDTO response,
            IArticleRepository articleRepository,
             IUploadFilesService uploadFilesService,
            IArticleImageRepository articleImageRepository,
            IUnitOfWork<AppDbContext> unitOfWork) : base(articleRepository, mapper)
        {
            _articleRepository = articleRepository;
            _articleImageRepository = articleImageRepository;
            _response = response;
            _uploadFilesService = uploadFilesService;
            _unitOfWork = unitOfWork;
        }


        public IResponseDTO GetAll(string rootPath, int? pageIndex = null, int? pageSize = null, ArticleFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.CMSSchema.Article> query = null;
            try
            {
                query = _articleRepository.GetAll()
                                    .Include(x => x.ArticleImages)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Title))
                    {
                        query = query.Where(x => x.Title.Trim().ToLower().Contains(filterDto.Title.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Content))
                    {
                        query = query.Where(x => x.Content.Trim().ToLower().Contains(filterDto.Content.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.ProvidedBy))
                    {
                        query = query.Where(x => x.ProvidedBy.Trim().ToLower().Contains(filterDto.ProvidedBy.Trim().ToLower()));
                    }
                }

                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ArticleDto>>(query.ToList());

                foreach (var item in dataList)
                {
                    item.ArticleImageDtos = item.ArticleImageDtos.Where(x => !x.IsDeleted).ToList();
                    foreach (var image in item.ArticleImageDtos.Where(x => !x.IsExternalResource))
                    {
                        if (!string.IsNullOrEmpty(image.AttachmentUrl))
                        {
                            image.AttachmentUrl = rootPath + image.AttachmentUrl;
                        }
                    }
                }

                _response.Data = new
                {
                    List = dataList,
                    Page = pageIndex ?? 0,
                    pageSize = pageSize ?? 0,
                    Total = total,
                    Pages = pageSize.HasValue && pageSize.Value > 0 ? total / pageSize : 1
                };

                _response.Message = "Ok";
                _response.IsPassed = true;

            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public IResponseDTO GetAllAsDrp(ArticleFilterDto filterDto = null)
        {
            try
            {
                var query = _articleRepository.GetAll(x => !x.IsDeleted && x.IsActive);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Title))
                    {
                        query = query.Where(x => x.Title.Trim().ToLower().Contains(filterDto.Title.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Content))
                    {
                        query = query.Where(x => x.Content.Trim().ToLower().Contains(filterDto.Content.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.ProvidedBy))
                    {
                        query = query.Where(x => x.ProvidedBy.Trim().ToLower().Contains(filterDto.ProvidedBy.Trim().ToLower()));
                    }
                }

                query = query.Select(i => new Data.DbModels.CMSSchema.Article() { Id = i.Id });
                var dataList = _mapper.Map<List<ArticleDto>>(query.ToList());

                _response.Data = dataList;
                _response.IsPassed = true;
                _response.Message = "Done";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public async Task<IResponseDTO> GetArticleDetails(string rootPath, int articleId)
        {
            try
            {
                var article = await _articleRepository.GetAll()
                                        .Include(x => x.ArticleImages)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == articleId);
                if (article == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                article.ArticleImages = article.ArticleImages.Where(x => !x.IsDeleted).ToList();
                var articleDto = _mapper.Map<ArticleDto>(article);

                foreach (var item in articleDto.ArticleImageDtos.Where(x => !x.IsExternalResource))
                {
                    if (!string.IsNullOrEmpty(item.AttachmentUrl))
                    {
                        item.AttachmentUrl = rootPath + item.AttachmentUrl;
                    }
                }

                _response.Data = articleDto;
                _response.Message = "Ok";
                _response.IsPassed = true;
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.Message = "Error " + ex.Message;
                _response.IsPassed = false;
            }

            return _response;
        }
        public async Task<IResponseDTO> CreateArticle(ArticleDto articleDto, List<IFormFile> files = null)
        {
            try
            {
                var article = _mapper.Map<Data.DbModels.CMSSchema.Article>(articleDto);


                // Add to the DB
                await _articleRepository.AddAsync(article);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                if (files != null)
                {
                    var path = $"\\Uploads\\Article\\Article_{article.Id}";
                    await _uploadFilesService.UploadFiles(path, files);
                    foreach (var item in article.ArticleImages.Where(x => !x.IsExternalResource && !x.IsDeleted))
                    {
                            item.AttachmentUrl = $"\\{path}\\{item.AttachmentName}";
                    }

                    _articleRepository.Update(article);
               
                    int finalSave = await _unitOfWork.CommitAsync();
                    if (finalSave == 0)
                    {
                        _response.Data = null;
                        _response.IsPassed = false;
                        _response.Message = "Not Saved";
                        return _response;
                    }
                }
               
                

                _response.Data = null;
                _response.IsPassed = true;
                _response.Message = "Ok";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public async Task<IResponseDTO> UpdateArticle(ArticleDto articleDto, List<IFormFile> files = null)
        {
            try
            {
                var articleExist = await _articleRepository.GetAll().Include(x => x.ArticleImages).FirstOrDefaultAsync(x => x.Id == articleDto.Id);
                if (articleExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }

                var article = _mapper.Map<Data.DbModels.CMSSchema.Article>(articleDto);

                // Fix URL
                foreach(var item in article.ArticleImages.Where(x => x.Id > 0))
                {
                    item.AttachmentUrl = articleExist.ArticleImages.First(x => x.Id == item.Id).AttachmentUrl;
                }

                if (files != null)
                {
                    var path = $"\\Uploads\\Article\\Article_{article.Id}";
                    await _uploadFilesService.UploadFiles(path, files);
                    foreach (var item in article.ArticleImages.Where(x => !x.IsExternalResource && !x.IsDeleted))
                    {
                        item.AttachmentUrl = $"\\{path}\\{item.AttachmentName}";
                    }
                }


                _articleRepository.Update(article);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.Data = null;
                _response.IsPassed = true;
                _response.Message = "Ok";

            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public async Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int articleId, bool IsActive)
        {
            try
            {
                var article = await _articleRepository.GetFirstAsync(x => x.Id == articleId);
                if (article == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsActive value
                article.IsActive = IsActive;
                article.UpdatedBy = loggedInUserId;
                article.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                article.ArticleImages = null;

                // Update on the Database
                _articleRepository.Update(article);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.IsPassed = true;
                _response.Message = "Ok";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }

            return _response;
        }
        public async Task<IResponseDTO> RemoveArticle(int articleId, int loggedInUserId)
        {
            try
            {
                var article = await _articleRepository.GetFirstOrDefaultAsync(x => x.Id == articleId);
                if (article == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsDeleted value
                article.IsDeleted = true;
                article.UpdatedBy = loggedInUserId;
                article.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                article.ArticleImages = null;

                // Update on the Database
                _articleRepository.Update(article);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.IsPassed = true;
                _response.Message = "Ok";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        // Validators methods
        public bool IsTitleUnique(ArticleDto articleDto)
        {
            var searchResult = _articleRepository.GetAll(x =>
                                              !x.IsDeleted
                                              && x.Id != articleDto.Id
                                              && x.Title.ToLower().Trim() == articleDto.Title.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }

    }
}
