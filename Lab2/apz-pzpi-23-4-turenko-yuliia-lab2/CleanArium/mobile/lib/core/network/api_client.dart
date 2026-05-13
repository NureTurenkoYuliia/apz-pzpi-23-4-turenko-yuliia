import 'package:dio/dio.dart';
import 'dart:developer';

class ApiClient {
  final Dio dio;

  ApiClient(String? token)
    : dio = Dio(
        BaseOptions(
          baseUrl: 'http://10.0.2.2:5250/api',
          headers: {
            'Content-Type': 'application/json',
            if (token != null) 'Authorization': 'Bearer $token',
          },
        ),
      ) {
    dio.interceptors.add(
      InterceptorsWrapper(
        onError: (e, handler) {
          log("API ERROR: ${e.response?.data}");
          handler.next(e);
        },
      ),
    );
  }
}
