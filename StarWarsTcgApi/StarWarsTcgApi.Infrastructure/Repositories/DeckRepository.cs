using Dapper;
using MySql.Data.MySqlClient;
using StarWarsTcgApi.Domain.Interfaces;
using StarWarsTcgApi.Domain.Models;
using StarWarsTcgApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Infrastructure.Repositories
{
    public class DeckRepository : GenericRepository<Deck, int>, IDeckRepository
    {
        public DeckRepository(MySqlDataAccess dataAccess) : base(dataAccess, "Deck") { }

        public async Task<IEnumerable<Deck>> GetDecksByUserIdAsync(Guid userId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE CreatedBy = @userId";
            return await connection.QueryAsync<Deck>(sql, new { userId = userId });
        }

        public async Task<Deck?> GetDeckWithCardsAsync(int deckId)
        {
            Deck deck = null;

            using (MySql.Data.MySqlClient.MySqlConnection connection = (MySqlConnection)_dataAccess.GetConnection())
            {
                await connection.OpenAsync();

                // Retrieve the Deck
                string deckQuery = "SELECT * FROM Deck WHERE Id = @deckId";
                using (var deckCommand = new MySqlCommand(deckQuery, connection))
                {
                    deckCommand.Parameters.AddWithValue("@deckId", deckId);
                    using (var reader = deckCommand.ExecuteReader())
                    {
                        if (await reader.ReadAsync())
                        {
                            deck = new Deck
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                                CreatedBy = reader.GetGuid("CreatedBy"),
                                CreatedAt = reader.GetDateTime("CreatedAt"),
                                LastUpdated = reader.GetDateTime("LastUpdated"),
                                IsPublic = reader.GetBoolean("IsPublic"),
                                DeckCards = []
                            };
                        }
                    }
                }

                // Retrieve the Card IDs contained with the Deck
                if (deck != null)
                {
                    string deckCardsQuery = "SELECT * FROM Deck_Cards WHERE DeckId = @deckId";
                    using (var deckCardsCommand = new MySqlCommand(deckCardsQuery, connection))
                    {
                        deckCardsCommand.Parameters.AddWithValue("@deckId", deckId);
                        using (var reader = deckCardsCommand.ExecuteReader())
                        {
                            while (await reader.ReadAsync())
                            {
                                var deckCard = new DeckCard
                                {
                                    Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? (int?)null : reader.GetInt32("Id"),
                                    DeckId = reader.GetInt32("DeckId"),
                                    CardId = reader.GetInt32("CardId"),
                                    Quantity = reader.GetInt32("Quantity")
                                };
                                deck.DeckCards.Add(deckCard);
                            }
                        }
                    }
                }
            }

            return deck;
        }

        public async Task<Deck> Create(Deck newDeck)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            using var transaction = connection.BeginTransaction();
            try
            {
                var deckSql = @"
                    INSERT INTO {_tableName} (Name, Description, CreatedAt, LastUpdated, IsValid, CreatedBy, IsPublic)
                    VALUES (@Name, @Description, @CreatedAt, @LastUpdated, @IsValid, @CreatedBy, @IsPublic);
                    SELECT LAST_INSERT_ID();
                ";
                newDeck.CreatedAt = DateTime.UtcNow;
                newDeck.LastUpdated = DateTime.UtcNow;

                var deckId = await connection.ExecuteScalarAsync<int>(deckSql, newDeck, transaction: transaction);
                newDeck.Id = deckId;

                var selectSql = "SELECT * FROM {_tableName} where Id = @Id;";
                var createdDeckFromDb = await connection.QuerySingleOrDefaultAsync<Deck>(selectSql, new { Id = newDeck.Id }, transaction: transaction);

                if (newDeck.DeckCards?.Count > 0)
                {
                    foreach (DeckCard dc in newDeck.DeckCards)
                    {
                        dc.DeckId = newDeck.Id.Value;
                        await AddDeckCardAsync(connection, transaction, dc);
                    }
                }
                transaction.Commit();
                return createdDeckFromDb ?? newDeck;
            }
            catch (Exception ex) // Catch specific exception for better error handling
            {
                transaction.Rollback(); // Rollback on error
                Console.WriteLine($"Error creating deck: {ex.Message}"); // Log the error
                throw; // Re-throw the exception for higher layers to handle
            }
        }
        #region DeckCard Management Methods
        public async Task AddDeckCardAsync(DeckCard deckCard)
        {
            string sql = $"INSERT INTO Deck_Cards (deckId, cardId, quantity) VALUES (@deckid, @cardId, @quantity)";
            using IDbConnection connection = _dataAccess.GetConnection();
            await connection.ExecuteAsync(sql, new { deckId = deckCard.DeckId, cardId = deckCard.CardId, quantity = deckCard.Quantity });
        }
        private async Task AddDeckCardAsync(IDbConnection connection, IDbTransaction transaction, DeckCard deckCard)
        {
            var deckCardSql = @"
                INSERT INTO swtcg.Deck_Cards (DeckId, CardId, Quantity)
                VALUES (@DeckId, @CardId, @Quantity);
            ";
            await connection.ExecuteAsync(deckCardSql, deckCard, transaction: transaction);
        }
        public async Task UpdateDeckCardAsync(DeckCard deckCard)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"UPDATE Deck_Cards SET Quantity = @quantity WHERE DeckId = @deckId AND CardId = @cardId";
            await connection.ExecuteAsync(sql, new { deckId = deckCard.DeckId, cardId = deckCard.CardId, quantity = deckCard.Quantity });
        }

        public async Task DeleteDeckCardAsync(int deckCardId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"DELETE FROM Deck_Cards WHERE Id = @deckCardId";
            await connection.ExecuteAsync(sql, new { deckCardId = deckCardId });
        }

        public async Task DeleteDeckAsync(int deckId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"DELETE FROM {_tableName} WHERE DeckId = @deckId";
            await connection.ExecuteAsync(sql, new { DeckId = @deckId });
        }

        #endregion
    }
}