using Microsoft.AspNetCore.Http;
using SimReport.Entities.Block;
using SimReport.Entities.Cards;
using SimReport.Interfaces;
using SimReport.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimReport.Services;

public class BlockService : IBlockService
{
    private readonly IRepository<BlockDate> blockRepository;

    public BlockService(IRepository<BlockDate> blockRepository)
    {
        this.blockRepository = blockRepository;
    }

    public async Task<Response<BlockDate>> AddAsync(BlockDate date)
    {
        try
        {
            await this.blockRepository.CreateAsync(date);
            await this.blockRepository.SaveChanges();

            return new Response<BlockDate>
            {
                StatusCode = 200,
                Message = "Ok",
                Data = date
            };
        }
        catch (Exception ex)
        {
            return new Response<BlockDate>
            {
                StatusCode = 403,
                Message = ex.Message,
                Data = null
            };
        }

    }

    public Task<Response<bool>> DeleteAsync(BlockDate date)
    {
        throw new System.NotImplementedException();
    }

    public async Task<Response<IEnumerable<BlockDate>>> GetAllAsync()
    {
        var dates = this.blockRepository.GetAll().ToList();
        return new Response<IEnumerable<BlockDate>>
        {
            StatusCode = 200,
            Message = "Success",
            Data = dates
        };
    }

    public async Task<Response<BlockDate>> UpdateAsync()
    {
        try
        {
            var date = this.blockRepository.GetAll().ToList()[0];
            date.EndDate = DateTime.UtcNow.AddYears(2);

            this.blockRepository.Update(date);
            await this.blockRepository.SaveChanges();

            return new Response<BlockDate>
            {
                StatusCode = 200,
                Message = "Success",
                Data = date
            };
        }
        catch (Exception ex)
        {
            return new Response<BlockDate>
            {
                StatusCode = 403,
                Message = ex.Message,
                Data = null
            };
        }
    }
}
