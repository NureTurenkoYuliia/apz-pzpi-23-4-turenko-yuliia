import 'package:dio/dio.dart';
import '../../data/models/scheduled_command_model.dart';

class CommandApi {
  final Dio dio;

  CommandApi(this.dio);

  Future<List<ScheduledCommand>> getByDevice(int deviceId) async {
    final res =
        await dio.get('/scheduledcommand/get-all-by-device/$deviceId');

    return (res.data as List)
        .map((e) => ScheduledCommand.fromJson(e))
        .toList();
  }

  Future<void> create(int deviceId, int commandType, DateTime startTime, int repeatMode, int? intervalMinutes, bool isActive) async {
    await dio.post('/scheduledcommand/create', data: {
      'deviceId': deviceId,
      'commandType': commandType,
      'startTime': startTime.toIso8601String(),
      'repeatMode': repeatMode,
      'intervalMinutes': intervalMinutes,
      'isActive': isActive,
    });
  }

  Future<void> update(int id, int commandType, DateTime startTime, int repeatMode, int? intervalMinutes, bool isActive,) async {
    await dio.put('/scheduledcommand/update', data: {
      'id': id,
      'commandType': commandType,
      'startTime': startTime.toIso8601String(),
      'repeatMode': repeatMode,
      'intervalMinutes': intervalMinutes,
      'isActive': isActive,
    });
  }

  Future<void> delete(int id) async {
    await dio.delete('/scheduledcommand/delete/$id');
  }

  Future<void> executeNow(int deviceId, int commandType) async {
    await dio.post('/device/$deviceId/executed-command', data: {
      'commandType': commandType,
      'commandStatus': 1,
    });
  }
}