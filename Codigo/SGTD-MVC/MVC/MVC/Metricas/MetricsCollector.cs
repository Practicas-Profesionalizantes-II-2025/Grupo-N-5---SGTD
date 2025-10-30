using Prometheus;

public static class MetricsCollector
{
    public static readonly Counter SalesCounter = Metrics.CreateCounter(
        "store_sales_total",
        "Total de ventas procesadas",
        new CounterConfiguration
        {
            LabelNames = new[] { "user_id" }
        });

    public static readonly Gauge ProductsInStock = Metrics.CreateGauge(
        "store_products_in_stock",
        "Unidades en stock por producto",
        new GaugeConfiguration
        {
            LabelNames = new[] { "rubro", "producto_id" }
        });

    public static readonly Histogram HttpRequestDuration = Metrics.CreateHistogram(
        "store_http_request_duration_seconds",
        "Duración de requests HTTP",
        new HistogramConfiguration
        {
            LabelNames = new[] { "method", "endpoint", "status_code" },
            Buckets = Histogram.ExponentialBuckets(0.01, 2, 10)
        });
}
