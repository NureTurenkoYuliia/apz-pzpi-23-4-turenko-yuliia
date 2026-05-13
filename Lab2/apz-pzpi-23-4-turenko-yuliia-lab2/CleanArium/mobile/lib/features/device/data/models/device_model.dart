class Device {
  final int id;
  final int aquariumId;
  final int deviceType;
  final int deviceStatus;

  Device({
    required this.id,
    required this.aquariumId,
    required this.deviceType,
    required this.deviceStatus,
  });

  factory Device.fromJson(Map<String, dynamic> json) {
    return Device(
      id: json['id'],
      aquariumId: json['aquariumId'],
      deviceType: json['deviceType'],
      deviceStatus: json['deviceStatus'],
    );
  }
}