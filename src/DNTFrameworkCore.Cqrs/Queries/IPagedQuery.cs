﻿using DNTFrameworkCore.Querying;

namespace DNTFrameworkCore.Cqrs.Queries
{
    public interface IPagedQuery<out TReadModel> : IQuery<IPagedResult<TReadModel>>, IPagedRequest
    {
    }

    public abstract class PagedQuery<TReadModel> : PagedRequest, IPagedQuery<TReadModel>
    {
    }
}