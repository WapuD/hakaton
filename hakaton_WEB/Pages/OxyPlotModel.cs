using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;

namespace hakaton_WEB.Pages
{
    public class OxyPlotModel
    {
        public static PlotModel CreatePlotModel(string employeeName, string competencyName, List<int> scores, List<DateTime> dates)
        {
            var plotModel = new PlotModel { Title = $"{employeeName} - {competencyName}" };
            var lineSeries = new LineSeries { Title = "Оценки", MarkerType = MarkerType.Circle };

            for (int i = 0; i < scores.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(dates[i]), scores[i]));
            }

            plotModel.Series.Add(lineSeries);
            return plotModel;
        }
    }
}
