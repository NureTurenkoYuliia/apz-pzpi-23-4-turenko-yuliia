import 'package:flutter/material.dart';
import 'package:easy_localization/easy_localization.dart';

class LanguageButton extends StatelessWidget {
  final VoidCallback? onChanged;
  const LanguageButton({super.key, this.onChanged});

  @override
  Widget build(BuildContext context) {
    return PopupMenuButton<Locale>(
      icon: const Icon(Icons.language),
      onSelected: (locale) async {
        await context.setLocale(locale);

        onChanged?.call();
      },
      itemBuilder: (_) => [
        const PopupMenuItem(
          value: Locale('en'),
          child: Text("English"),
        ),
        const PopupMenuItem(
          value: Locale('uk'),
          child: Text("Українська"),
        ),
      ],
    );
  }
}