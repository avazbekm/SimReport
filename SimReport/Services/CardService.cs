using System;
using System.Linq;
using SimReport.Interfaces;
using System.Threading.Tasks;
using SimReport.Entities.Cards;
using SimReport.Entities.Users;
using SimReport.Services.Helpers;
using System.Collections.Generic;
using SimReport.Entities.Companies;
using SimReport.Services.Exceptions;

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
            var cards = cardRepository.GetAll(a => a.CompanyId.Equals(card.CompanyId)).ToList();
            if (cards is not null)
            {
                foreach (var item in cards)
                {
                    if (item.UserId.Equals(card.UserId) &&
                        item.CardNumber.Equals(card.CardNumber))
                        throw new AlreadyExistException("Bu seriya bilan sim karta mavjud.");

                    if (!item.UserId.Equals(card.UserId) &&
                        item.CardNumber.Equals(card.CardNumber))
                        throw new AlreadyExistException("Bu seriya bilan sim karta mavjud.");
                }
            }
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

    public async Task<Response<bool>> DeleteAsync(Card card)
    {
        var cards = cardRepository.GetAll(a => a.CompanyId.Equals(card.CompanyId)).ToList();
        if (cards.Count > 0)
        {
            foreach (var item in cards)
            {
                if (item.CardNumber.Equals(card.CardNumber))
                {
                    item.Comment = card.Comment;

                    this.cardRepository.Delete(item);
                    await this.cardRepository.SaveChanges();
                    return new Response<bool>
                    {
                        StatusCode = 200,
                        Message = "Ok",
                        Data = true
                    };
                }
            }   
        }

        return new Response<bool>
        {
            StatusCode = 403,
            Message = "Topilmadi",
            Data = false
        };
    }

    public async Task<Response<IEnumerable<Card>>> GetAllAsync()
    {
        var cards = this.cardRepository.GetAll().ToList();
        return new Response<IEnumerable<Card>>
        {
            StatusCode = 200,
            Message = "Ok",
            Data = cards
        };
    }
    public async Task<Response<IEnumerable<Card>>> GetAllAsync(int companyId)
    {
        try
        {
            var cards = cardRepository.GetAll(a => a.CompanyId.Equals(companyId)).ToList();
            if (cards.Count > 0)
                return new Response<IEnumerable<Card>>
                {
                    StatusCode = 200,
                    Message = "Ok",
                    Data = cards
                };

        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<Card>>
            {
                StatusCode = 403,
                Message = ex.Message,
                Data = null
            };
        }

        return new Response<IEnumerable<Card>>
        {
            StatusCode = 403,
            Message = "Bu companiyaga biriktirilgan sim cartalar mavjud emas.",
            Data = null
        };

    }

    public async Task<Response<Card>> GetAsync(long id)
    {
        try
        {
            var existCard = await this.cardRepository.GetAsync(u => u.Id.Equals(id));
            if (existCard is null)
                throw new NotFoundException("Bunday seria bilan sim karta mavjud emas.");

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

    public async Task<Response<bool>> ReturnAsync(long seriaNum, int id,string comment)
    {
        var cards = cardRepository.GetAll(a => a.CompanyId.Equals(id)).ToList();
        if (cards.Count > 0)
        {
            foreach (var item in cards)
            {
                if (item.CardNumber.Equals(seriaNum))
                {
                    item.Comment = comment;
                    item.IsReturn = true;

                    this.cardRepository.Delete(item);
                    await this.cardRepository.SaveChanges();
                    return new Response<bool>
                    {
                        StatusCode = 200,
                        Message = "Ok",
                        Data = true
                    };
                }
            }
        }

        return new Response<bool>
        {
            StatusCode = 403,
            Message = "Topilmadi",
            Data = false
        };
    }

    public async Task<Response<Card>> UpdateAsync(Card Card)
    {
        try
        {
            var existCard = await this.cardRepository.GetAsync(u => u.CardNumber.Equals(Card.CardNumber));
            if (existCard != null)
                throw new AlreadyExistException("Bunday nomer foydalanuvchisi mavjud emas!");

            existCard.UserId = Card.UserId;
            existCard.CompanyId = Card.CompanyId;
            existCard.CardNumber = Card.CardNumber;

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
