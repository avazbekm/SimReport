using System;
using System.Linq;
using SimReport.Interfaces;
using System.Threading.Tasks;
using SimReport.Entities.Cards;
using SimReport.Services.Helpers;
using System.Collections.Generic;
using SimReport.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace SimReport.Services;

public class CardService : ICardService
{
    private readonly IRepository<Card> cardRepository;
    public CardService(IRepository<Card> cardRepository)
    {

        this.cardRepository = cardRepository;
    }
    public async Task<Response<Card>> AddAsync(Card card)
    {
        try
        {
            var existCard = this.cardRepository.GetAll().FirstOrDefault(c => c.CardNumber.Equals(card.CardNumber));
            if (existCard is not null)
                throw new AlreadyExistException("Bu seriya bilan sim karta mavjud.");

            await this.cardRepository.CreateAsync(card);
            await this.cardRepository.SaveChanges();

            return new Response<Card>
            {
                StatusCode = 200,
                Message = "Ok",
                Data = card
            };
        }
        catch (Exception ex)
        {
            return new Response<Card>
            {
                StatusCode = 403,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<Response<bool>> DeleteAsync(long id)
    {
        var existCard = this.cardRepository.GetAll().FirstOrDefault(u => u.Id.Equals(id));
        if (existCard is null)
            throw new NotFoundException("Bu seria bilan sim karta mavjud emas.");
        else
        {
            this.cardRepository.Delete(existCard);
            await this.cardRepository.SaveChanges();
            return new Response<bool>
            {
                StatusCode = 200,
                Message = "Ok",
                Data = true
            };
        }
    }

    public async Task<Response<IEnumerable<Card>>> GetAllAsync()
    {
        var cards = await this.cardRepository.GetAll().ToListAsync();
        return new Response<IEnumerable<Card>>
        {
            StatusCode = 200,
            Message = "Ok",
            Data = cards
        };
    }

    public async Task<Response<Card>> GetAsync(long id)
    {
        try
        {
            var existCard = await this.cardRepository.GetAll().FirstOrDefaultAsync(u => u.Id.Equals(id));
            if (existCard is null)
                throw new NotFoundException("Buday seria bilan sim karta mavjud emas.");

            return new Response<Card>
            {
                StatusCode = 200,
                Message = "Ok",
                Data = existCard
            };
        }
        catch (Exception ex)
        {
            return new Response<Card>
            {
                StatusCode = 403,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<Response<Card>> UpdateAsync(Card Card)
    {
        try
        {
            var existCard = await this.cardRepository.GetAll().FirstOrDefaultAsync(u => u.CardNumber.Equals(Card.CardNumber));
            if (existCard != null)
                throw new AlreadyExistException("Bunday nomer foydalanuvchisi mavjud emas!");

            existCard.UserId = Card.UserId;
            existCard.CompanyId = Card.CompanyId;
            existCard.CardNumber= Card.CardNumber;

            this.cardRepository.Update(Card);
            await this.cardRepository.SaveChanges();

            return new Response<Card>
            {
                StatusCode = 200,
                Message = "Ok",
                Data = Card
            };
        }
        catch (Exception ex)
        {
            return new Response<Card>
            {
                StatusCode = 403,
                Message = ex.Message,
                Data = null
            };
        }
    }
}
