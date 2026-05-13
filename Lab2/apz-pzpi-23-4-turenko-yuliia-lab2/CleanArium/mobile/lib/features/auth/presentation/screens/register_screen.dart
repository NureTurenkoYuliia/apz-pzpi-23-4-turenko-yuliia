import 'package:flutter/material.dart';
import 'package:dio/dio.dart';
import '../../../../core/utils/token_storage.dart';
import '../../../../core/network/api_client.dart';
import '../../../home/presentation/screens/home_screen.dart';
import '../screens/login_screen.dart';
import '../../../../shared/widgets/language_button.dart';
import 'package:easy_localization/easy_localization.dart';

class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  late Dio api;
  bool obscurePassword = true;

  @override
  void initState() {
    super.initState();
    init();
  }

  Future<void> init() async {
    final token = await TokenStorage().getAccessToken();
    api = ApiClient(token).dio;
  }

  final emailController = TextEditingController();
  final passwordController = TextEditingController();
  final userNameController = TextEditingController();

  final _formKey = GlobalKey<FormState>();
  final tokenStorage = TokenStorage();

  bool isLoading = false;

  Future<void> register() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => isLoading = true);

    try {
      final response = await api.post(
        '/auth/register',
        data: {
          'userName': userNameController.text.trim(),
          'email': emailController.text.trim(),
          'password': passwordController.text.trim(),
        },
      );

      if (!mounted) return;

      final access = response.data['accessToken'];
      final refresh = response.data['refreshToken'];

      await tokenStorage.saveTokens(access, refresh);

      if (!mounted) return;

      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text("auth_registration_success".tr())));

      Navigator.pushReplacement(
        context,
        MaterialPageRoute(builder: (_) => const HomeScreen()),
      );
    } on DioException catch (e) {
      String message = "auth_registration_failed".tr();

      if (e.response != null && e.response!.data != null) {
        message = e.response!.data.toString();
      }

      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(message)));
    } catch (_) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text("unexpected_error".tr())));
    } finally {
      setState(() => isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("register".tr()),
        actions: [
          LanguageButton(
            onChanged: () {
              setState(() {});
            },
          ),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Text(
                "CleanArium",
                style: TextStyle(
                  fontSize: 28,
                  color: Colors.lightBlueAccent,
                  fontWeight: FontWeight.bold,
                ),
              ),

              const SizedBox(height: 40),

              TextFormField(
                controller: userNameController,
                decoration: InputDecoration(labelText: "name".tr()),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return "validation_enter_name".tr();
                  }
                  return null;
                },
              ),

              const SizedBox(height: 16),

              TextFormField(
                controller: emailController,
                decoration: InputDecoration(labelText: "email".tr()),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return "validation_enter_email".tr();
                  }
                  if (!value.contains('@')) {
                    return "validation_invalid_email".tr();
                  }
                  return null;
                },
              ),

              const SizedBox(height: 16),

              TextFormField(
                controller: passwordController,
                obscureText: obscurePassword,
                decoration: InputDecoration(
                  labelText: "password".tr(),
                  suffixIcon: IconButton(
                    icon: Icon(
                      obscurePassword ? Icons.visibility_off : Icons.visibility,
                      color: Colors.blueGrey,
                    ),
                    onPressed: () {
                      setState(() {
                        obscurePassword = !obscurePassword;
                      });
                    },
                  ),
                ),
                validator: (value) {
                  if (value == null || value.length < 6) {
                    return "validation_min_chars".tr();
                  }
                  return null;
                },
              ),

              const SizedBox(height: 24),

              isLoading
                  ? const CircularProgressIndicator()
                  : ElevatedButton(
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.lightBlue,
                      ),
                      onPressed: register,
                      child: Text("register".tr()),
                    ),

              TextButton(
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(builder: (_) => const LoginScreen()),
                  );
                },
                child: Text("sign_in".tr()),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
