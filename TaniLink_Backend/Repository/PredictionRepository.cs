using Microsoft.EntityFrameworkCore;
using TaniLink_Backend.Data;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Repository
{
    public class PredictionRepository : IPredictionRepository
    {
        private readonly ApplicationDbContext _context;

        public PredictionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Prediction> CreatePrediction(Prediction prediction)
        {
            var createPrediction = _context.Predictions.Add(prediction);
            await _context.SaveChangesAsync();
            return createPrediction.Entity;
        }

        public async Task<Prediction> DeletePrediction(string predictionId)
        {
            var prediction = await _context.Predictions
                .FirstOrDefaultAsync(p => p.Id == predictionId);

            if (typeof(Auditable).IsAssignableFrom(typeof(Prediction)))
            {
                (prediction as Auditable).DeletedAt = DateTimeOffset.UtcNow;
                _context.Predictions.Attach(prediction);
                _context.Entry(prediction).State = EntityState.Modified;
            }
            else
            {
                _context.Predictions.Remove(prediction);
            }

            await _context.SaveChangesAsync();
            return prediction;
        }

        public async Task<IEnumerable<Prediction>> GetAllPredictions()
        {
            var predictions = await _context.Predictions
                .Include(p => p.Commodity)
                .ToListAsync();
            return predictions;
        }

        public Task<Prediction> GetByPredictionId(string predictionId)
        {
            var prediction = _context.Predictions
                .Include(p => p.Commodity)
                .FirstOrDefaultAsync(p => p.Id == predictionId);
            return prediction;
        }

        public async Task<Prediction> UpdatePrediction(Prediction prediction)
        {
            var updatePrediction = _context.Predictions.Update(prediction);
            await _context.SaveChangesAsync();
            return updatePrediction.Entity;
        }
    }
}
