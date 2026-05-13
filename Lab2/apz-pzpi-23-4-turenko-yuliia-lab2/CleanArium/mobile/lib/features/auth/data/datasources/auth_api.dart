import 'package:dio/dio.dart';

class AuthApi {
  final Dio dio;

  AuthApi(this.dio);

  Future<Map<String, dynamic>> login(String email, String password) async {
    final response = await dio.post('/auth/login', data: {
      'email': email,
      'password': password,
    });

    return response.data;
  }

  Future<void> register(
      String username, String email, String password) async {
    await dio.post('/auth/register', data: {
      'userName': username,
      'email': email,
      'password': password,
    });
  }
}