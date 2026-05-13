class ScheduledCommand {
  final int id;
  final int deviceId;
  final int commandType;
  final DateTime startTime;
  final int repeatMode;
  final int? intervalMinutes;
  final bool isActive;

  ScheduledCommand({
    required this.id,
    required this.deviceId,
    required this.commandType,
    required this.startTime,
    required this.repeatMode,
    required this.intervalMinutes,
    required this.isActive,
  });

  factory ScheduledCommand.fromJson(Map<String, dynamic> json) {
    return ScheduledCommand(
      id: json['id'],
      deviceId: json['deviceId'],
      commandType: json['commandType'],
      startTime: DateTime.parse(json['startTime']),
      repeatMode: json['repeatMode'],
      intervalMinutes: json['intervalMinutes'],
      isActive: json['isActive'],
    );
  }
}