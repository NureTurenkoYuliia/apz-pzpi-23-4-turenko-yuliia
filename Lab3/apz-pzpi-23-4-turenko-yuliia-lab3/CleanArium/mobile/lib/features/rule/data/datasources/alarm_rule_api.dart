import 'package:dio/dio.dart';
import '../../data/models/alarm_rule_model.dart';

class AlarmRuleApi {
  final Dio dio;

  AlarmRuleApi(this.dio);

  Future<List<AlarmRule>> getByDevice(int deviceId) async {
    final res = await dio.get('/alarmrule/get-all-by-device/$deviceId');

    return (res.data as List)
        .map((e) => AlarmRule.fromJson(e))
        .toList();
  }

  Future<void> create(int deviceId, int condition, double threshold, String unit ) async {
    await dio.post('/alarmrule/create', data: {
      'deviceId': deviceId,
      'condition': condition,
      'threshold': threshold,
      'unit': unit,
    });
  }

  Future<void> update(int ruleId, int condition, double threshold, String unit, bool isActive) async {
    await dio.put('/alarmrule/update', data: {
      'ruleId': ruleId,
      'condition': condition,
      'threshold': threshold,
      'unit': unit,
      'isActive': isActive,
    });
  }

  Future<void> delete(int ruleId) async {
    await dio.delete('/alarmrule/delete', data: ruleId);
  }
}