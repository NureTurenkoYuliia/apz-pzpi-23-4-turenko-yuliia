import 'package:flutter/material.dart';
import 'features/auth/presentation/screens/login_screen.dart';
import 'core/theme/app_theme.dart';
import 'package:easy_localization/easy_localization.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await EasyLocalization.ensureInitialized();

  runApp(
    EasyLocalization(
      supportedLocales: const [Locale('en'), Locale('uk')],
      path: 'lib/assets/translations',
      fallbackLocale: const Locale('en'),
      child: const CleanAriumApp(),
    ),
     );
}

class CleanAriumApp extends StatelessWidget {
  const CleanAriumApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'CleanArium',
      theme: AppTheme.theme,
      home: const LoginScreen(),
      locale: context.locale,
      supportedLocales: context.supportedLocales,
      localizationsDelegates: context.localizationDelegates,
    );
  }
}