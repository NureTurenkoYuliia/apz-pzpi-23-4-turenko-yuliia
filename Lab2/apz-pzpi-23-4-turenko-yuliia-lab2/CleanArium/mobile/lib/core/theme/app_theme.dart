import 'package:flutter/material.dart';

class AppTheme {
  static const primary = Color(0xFF0B2545);
  static const accent = Color(0xFF96705B);
  static const bgLight = Color(0xFFEEF4ED);
  static const bgCard = Color(0xFFDBE7F1);

  static ThemeData theme = ThemeData(
    scaffoldBackgroundColor: const Color(0xFFEEF4ED), // або DBE7F1

    appBarTheme: const AppBarTheme(
      backgroundColor: primary,
      foregroundColor: bgLight,
    ),

    cardTheme: CardThemeData(
      color: bgCard,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
    ),

    textTheme: const TextTheme(bodyMedium: TextStyle(color: Colors.black)),

    inputDecorationTheme: const InputDecorationTheme(
      labelStyle: TextStyle(color: Colors.black54),
      hintStyle: TextStyle(color: Colors.black45),
      enabledBorder: UnderlineInputBorder(
        borderSide: BorderSide(color: Colors.black26),
      ),
      focusedBorder: UnderlineInputBorder(
        borderSide: BorderSide(color: Color(0xFF0B2545)),
      ),
    ),
  );
}
