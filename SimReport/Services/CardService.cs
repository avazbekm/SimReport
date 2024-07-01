using System;
using System.Linq;
using SimReport.Interfaces;
using System.Threading.Tasks;
using SimReport.Entities.Cards;
using SimReport.Entities.Users;
using SimReport.Services.Helpers;
using System.Collections.Generic;
using SimReport.Services.Exceptions;

namespace SimReport.Services;

public class CardService : ICardService
{
    private readonly IRepository<Card> cardRepository;
    private readonly IRepository<User> userRepository;

    public CardService(IRepository<Card> cardRepository, IRepository<User> userRepository)
    {
        this.cardRepository = cardRepository;
        this.userRepository = userRepository;
    }
    public async Task<Response<Card>> AddAsync(Card card)
    {
        try
        {
            var cards = cardRepository.GetAll(a => a.CompanyId.Equals(card.CompanyId)).ToList();
            if (cards.Count>0)
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
                    item.IsReturn = card.IsReturn;

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

    public async Task<Response<bool>> DeleteAsync(int id, string first, string last)
    {
        var card = await this.cardRepository.GetAsync(a => a.Id.Equals(id));
        if (card is not null)
        {
            card.IsReturn = true;
            card.Comment = $"{last} {first}ga berilgan.";

            this.cardRepository.Delete(card);
            await this.cardRepository.SaveChanges();
            return new Response<bool>
            {
                StatusCode = 200,
                Message = "Ok",
                Data = true
            };
        }
        return new Response<bool>
        {
            StatusCode = 403,
            Message = "Xatolik",
            Data = false
        };
    }

    public async Task<Response<bool>> DeleteAsync(int id, string comment)
    {
        var card = await this.cardRepository.GetAsync(a => a.Id.Equals(id));
        card.IsReturn = true;
        card.Comment = comment;

        this.cardRepository.Delete(card);
        await this.cardRepository.SaveChanges();
        return new Response<bool>
        {
            StatusCode = 200,
            Message = "Ok",
            Data = true
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

    public async Task<Response<IEnumerable<Card>>> GetAllAsync(int companyId, string first, string last)
    {
        try
        {
            var cards = cardRepository.GetAll(a => 
            a.CompanyId.Equals(companyId) &&
            a.User.FirstName.Equals(first) &&
            a.User.LastName.Equals(last))
                .ToList();

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

    public async Task<Response<IEnumerable<Card>>> GetAllAsync(string phone)
    {
        try
        {
            var cards = cardRepository.GetAll(a =>a.User.Phone.Equals(phone)).ToList();

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
            Message = "Bu hamkorga biriktirilgan sim cartalar mavjud emas.",
            Data = null
        };
    }

    public async Task<Response<Card>> GetAsync(int id)
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

    public async Task<Response<Card>> GetAsync(long seriaNumber)
    {
        try
        {
            var cards = cardRepository.GetAll(a => a.CardNumber.Equals(seriaNumber)).ToList();

            if (cards.Count == 0)
                throw new NotFoundException("Bunday seria bilan sim karta mavjud emas.");

            return new Response<Card>
            {
                StatusCode = 200,
                Message = "Ok",
                Data = cards.Last()
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

    public async Task<Response<bool>> ReturnAsync(long seriaNum, int id)
    {
        var user = await this.userRepository.GetAsync(u=>u.FirstName.Equals("asosiy") && u.LastName.Equals("baza"));

        Card card = new Card();
        
        card.Id = 0;
        card.UserId = user.Id;
        card.CompanyId = id;
        card.CardNumber = seriaNum;

        await this.cardRepository.CreateAsync(card);
        await this.cardRepository.SaveChanges();
        return new Response<bool>
        {
            StatusCode = 200,
            Message = "Ok",
            Data = true
        };
    }

    public async Task<Response<Card>> SellAsync(Card card)
    {
        try
        {
            var existCard = await this.cardRepository.GetAsync(u => u.Id.Equals(card.Id));
            if (existCard is null)
                throw new NotFoundException("Bunday seriali sim karta mavjud emas!");

            existCard.SoldTime = card.SoldTime;
            existCard.IsSold = true;
            existCard.IsDeleted = true;
            existCard.Comment = card.Comment;


            this.cardRepository.Update(existCard);
            await this.cardRepository.SaveChanges();

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

    public async Task<Response<Card>> TransferAsync(Card card)
    {
        try 
        { 
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

    public async Task<Response<Card>> UpdateAsync(Card Card)
    {
        try
        {
            var existCard = await this.cardRepository.GetAsync(u => u.Id.Equals(Card.Id));
            if (existCard is null)
                throw new NotFoundException("Bunday seriali sim karta mavjud emas!");

            existCard.UserId = Card.UserId;

            this.cardRepository.Update(existCard);
            await this.cardRepository.SaveChanges();

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
}
