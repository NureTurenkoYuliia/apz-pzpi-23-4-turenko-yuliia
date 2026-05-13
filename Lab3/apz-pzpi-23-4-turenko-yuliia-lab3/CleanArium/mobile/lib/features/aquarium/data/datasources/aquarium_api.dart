import 'package:dio/dio.dart';
import '../models/aquarium_model.dart';

class AquariumApi {
  final Dio dio;

  AquariumApi(this.dio);

  Future<List<Aquarium>> getAll() async {
    final res = await dio.get('/aquarium/get-all-by-user');

    return (res.data as List)
        .map((e) => Aquarium.fromJson(e))
        .toList();
  }

  Future<void> create(String name, String? location) async {
    await dio.post('/aquarium/create', data: {
      'name': name,
      'location': location,
    });
  }

  Future<void> update(int id, String name, String? location) async {
    await dio.put('/aquarium/update', data: {
      'aquariumId': id,
      'name': name,
      'location': location,
    });
  }

  Future<void> delete(int id) async {
    await dio.delete('/aquarium/delete', data: id);
  }
}