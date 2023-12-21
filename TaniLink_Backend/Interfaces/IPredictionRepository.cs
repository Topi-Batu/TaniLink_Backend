using TaniLink_Backend.Models;

namespace TaniLink_Backend.Interfaces
{
    public interface IPredictionRepository
    {
        Task<IEnumerable<Prediction>> GetAllPredictions();
        Task<IEnumerable<Prediction>> GetAllPredictionByCommodityIdAndAreaIdAndMonthAndYear(string commodityId, string areaId, DateOnly dateOnly);
        Task<Prediction> GetByPredictionId(string predictionId);
        Task<Prediction> GetByPredictionDate(DateOnly predictionDate);
        Task<Prediction> GetLatestPrediction();
        Task<Prediction> CreatePrediction(Prediction prediction);
        Task<Prediction> UpdatePrediction(Prediction prediction);
        Task<Prediction> DeletePrediction(string predictionId);
    }
}
