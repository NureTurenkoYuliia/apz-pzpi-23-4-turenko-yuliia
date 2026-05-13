class SensorData {
  final double value;
  final String unit;

  SensorData({
    required this.value,
    required this.unit,
  });

  factory SensorData.fromJson(Map<String, dynamic> json) {
    return SensorData(
      value: (json['value'] as num).toDouble(),
      unit: json['unit'],
    );
  }
}