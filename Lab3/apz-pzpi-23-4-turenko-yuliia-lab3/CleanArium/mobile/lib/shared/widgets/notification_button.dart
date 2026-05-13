import 'package:flutter/material.dart';
import '../../../../core/utils/token_storage.dart';
import '../../../../core/network/api_client.dart';
import 'package:easy_localization/easy_localization.dart';
import '../../features/notifications/data/datasources/notification_api.dart';
import '../../features/notifications/presentation/screens/notifications_screen.dart';

class NotificationButton extends StatefulWidget {
  const NotificationButton({super.key});

  @override
  State<NotificationButton> createState() => _NotificationButtonState();
}

class _NotificationButtonState extends State<NotificationButton> {
  int unread = 0;

  @override
  void initState() {
    super.initState();
    loadUnread();
  }

  Future<void> loadUnread() async {
    final token = await TokenStorage().getAccessToken();

    final api = NotificationApi(
      ApiClient(token).dio,
    );

    try {
      final count = await api.getUnreadCount();

      if (!mounted) return;

      setState(() {
        unread = count;
      });
    } catch (_) {}
  }

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        IconButton(
          icon: const Icon(Icons.notifications),
          tooltip: "notifications".tr(),
          onPressed: () async {
            await Navigator.push(
              context,
              MaterialPageRoute(
                builder: (_) => const NotificationsScreen(),
              ),
            );

            loadUnread();
          },
        ),

        if (unread > 0)
          Positioned(
            right: 6,
            top: 6,
            child: Container(
              padding: const EdgeInsets.all(5),
              decoration: const BoxDecoration(
                color: Colors.red,
                shape: BoxShape.circle,
              ),
              child: Text(
                unread.toString(),
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 10,
                ),
              ),
            ),
          ),
      ],
    );
  }
}