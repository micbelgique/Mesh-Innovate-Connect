public class PamesebData
{
    public List<PamesebHourlyData> Data { get; set; }
}

public class PamesebHourlyData
{
    public string Timestamp { get; set; }
    public double Tsa { get; set; }
    public double Plu { get; set; }
    public double Hra { get; set; }
}