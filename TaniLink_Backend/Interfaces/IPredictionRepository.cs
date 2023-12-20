using TaniLink_Backend.Models;

namespace TaniLink_Backend.Interfaces
{
    public interface IPredictionRepository
    {
        Task<IEnumerable<Prediction>> GetAllPredictions();
        Task<Prediction> GetByPredictionId(string predictionId);
        Task<Prediction> CreatePrediction(Prediction prediction);
        Task<Prediction> UpdatePrediction(Prediction prediction);
        Task<Prediction> DeletePrediction(string predictionId);
    }
}
