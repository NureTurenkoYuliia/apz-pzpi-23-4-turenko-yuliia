import 'package:dio/dio.dart';

import '../models/notification_model.dart';

class NotificationApi {
  final Dio dio;

  NotificationApi(this.dio);

  Future<List<AppNotification>> getAll() async {
    final res = await dio.get('/notification/get-all');

    return (res.data as List)
        .map((e) => AppNotification.fromJson(e))
        .toList();
  }

  Future<void> markAsRead(int id) async {
    await dio.post('/notification/read/$id');
  }

  Future<int> getUnreadCount() async {
    final res = await dio.get('/notification/unread-count');

    return res.data;
  }
}