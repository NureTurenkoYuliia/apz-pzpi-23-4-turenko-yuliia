class AlarmRule {
  final int id;
  final int deviceId;
  final int condition;
  final double threshold;
  final String unit;
  final bool isActive;

  AlarmRule({
    required this.id,
    required this.deviceId,
    required this.condition,
    required this.threshold,
    required this.unit,
    required this.isActive,
  });

  factory AlarmRule.fromJson(Map<String, dynamic> json) {
    return AlarmRule(
      id: json['id'],
      deviceId: json['deviceId'],
      condition: json['condition'],
      threshold: (json['threshold'] as num).toDouble(),
      unit: json['unit'],
      isActive: json['isActive'],
    );
  }
}