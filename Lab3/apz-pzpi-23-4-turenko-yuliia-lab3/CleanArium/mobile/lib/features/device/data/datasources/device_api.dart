import 'package:dio/dio.dart';
import '../../../device/data/models/device_model.dart';
import '../../../device/data/models/sensor_data_model.dart';

class DeviceApi {
  final Dio dio;

  DeviceApi(this.dio);

  Future<List<Device>> getByAquarium(int aquariumId) async {
    final res = await dio.get('/device/get-devices-by-aquarium/$aquariumId');

    return (res.data as List)
        .map((e) => Device.fromJson(e))
        .toList();
  }

  Future<SensorData?> getSensor(int deviceId) async {
    final res = await dio.get('/device/get-sensor-data/$deviceId')
    .timeout(const Duration(seconds: 3));

    if(res.data == null) return null;

    return SensorData.fromJson(res.data);
  }

  Future<void> create(int aquariumId, int deviceType) async {
    await dio.post('/device/create', data: {
      'aquariumId': aquariumId,
      'deviceType': deviceType,
      'deviceStatus': 2, // Off за замовчуванням
    });
  }

  Future<void> delete(int id) async {
    await dio.delete('/device/delete/$id');
  }

  Future<void> update(int id, int type, int status) async {
    await dio.put('/device/update', data: {
      'deviceId': id,
      'deviceType': type,
      'deviceStatus': status,
    });
  }
}