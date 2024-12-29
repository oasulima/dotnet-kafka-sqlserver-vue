using System.Data;
using LinqToDB;
using Microsoft.Data.SqlClient;
using Locator.API.Data.Entities;
using Locator.API.Data.Exceptions;
using Locator.API.Data.Extensions;
using Locator.API.Data.Repositories.Interfaces;
using Shared.Settings;

namespace Locator.API.Data.Repositories;

public class ProviderSettingRepository : IProviderSettingRepository
{
    private readonly DbConnection linq2Db;

    public ProviderSettingRepository(DbConnection linq2Db)
    {
        this.linq2Db = linq2Db;
    }

    public ProviderSetting? Get(string providerId)
    {
        return linq2Db.ProviderSettings
             .Where(x => x.ProviderId == providerId)
             .Select(x => new ProviderSetting
             {
                 ProviderId = x.ProviderId,
                 Active = x.Active,
                 BuyRequestTopic = x.BuyRequestTopic,
                 BuyResponseTopic = x.BuyResponseTopic,
                 Discount = x.Discount,
                 Name = x.Name,
                 QuoteRequestTopic = x.QuoteRequestTopic,
                 QuoteResponseTopic = x.QuoteResponseTopic,
             })
             .FirstOrDefault();
    }

    public List<ProviderSetting> GetAll()
    {
        return linq2Db.ProviderSettings
             .Select(x => new ProviderSetting
             {
                 ProviderId = x.ProviderId,
                 Active = x.Active,
                 BuyRequestTopic = x.BuyRequestTopic,
                 BuyResponseTopic = x.BuyResponseTopic,
                 Discount = x.Discount,
                 Name = x.Name,
                 QuoteRequestTopic = x.QuoteRequestTopic,
                 QuoteResponseTopic = x.QuoteResponseTopic,
             })
             .ToList();
    }

    /// <exception cref="UniqueConstraintViolationException"></exception>
    public void Add(ProviderSetting providerSetting)
    {
        var entity = new ProviderSettingDb
        {
            ProviderId = providerSetting.ProviderId,
            Active = providerSetting.Active,
            BuyRequestTopic = providerSetting.BuyRequestTopic,
            BuyResponseTopic = providerSetting.BuyResponseTopic,
            Discount = providerSetting.Discount,
            Name = providerSetting.Name,
            QuoteRequestTopic = providerSetting.QuoteRequestTopic,
            QuoteResponseTopic = providerSetting.QuoteResponseTopic,
        };

        try
        {
            linq2Db.Insert(entity);
        }
        catch (SqlException sqlException)
        {
            if (sqlException.IsPrimaryKeyConstraintViolation())
            {
                throw new UniqueConstraintViolationException("ProviderId", sqlException);
            }

            if (sqlException.IsUniqueKeyConstraintViolation())
            {
                throw new UniqueConstraintViolationException("Name", sqlException);
            }

            throw;
        }
    }

    public void Update(ProviderSetting providerSetting)
    {
        linq2Db.ProviderSettings
            .Where(x => x.ProviderId == providerSetting.ProviderId)
            .Set(x => x.Active, providerSetting.Active)
            .Set(x => x.BuyRequestTopic, providerSetting.BuyRequestTopic)
            .Set(x => x.BuyResponseTopic, providerSetting.BuyResponseTopic)
            .Set(x => x.Discount, providerSetting.Discount)
            .Set(x => x.Name, providerSetting.Name)
            .Set(x => x.QuoteRequestTopic, providerSetting.QuoteRequestTopic)
            .Set(x => x.QuoteResponseTopic, providerSetting.QuoteResponseTopic)
            .Update();
    }

    public void Delete(string providerId)
    {
        linq2Db.ProviderSettings
            .Where(x => x.ProviderId == providerId)
            .Delete();
    }
}
