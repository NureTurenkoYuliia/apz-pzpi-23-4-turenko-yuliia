class Aquarium {
  final int id;
  final int userId;
  final String name;
  final String? location;
  final bool isActive;

  Aquarium({
    required this.id,
    required this.userId,
    required this.name,
    this.location,
    required this.isActive,
  });

  factory Aquarium.fromJson(Map<String, dynamic> json) {
    return Aquarium(
      id: json['id'],
      userId: json['userId'],
      name: json['name'],
      location: json['location'],
      isActive: json['isActive'],
    );
  }
}