import 'package:flutter/material.dart';
import '../../../../core/utils/token_storage.dart';
import '../../../../core/network/api_client.dart';
import 'package:easy_localization/easy_localization.dart';
import '../../data/datasources/notification_api.dart';
import '../../data/models/notification_model.dart';
import '../../../../shared/widgets/language_button.dart';

class NotificationsScreen extends StatefulWidget {
  const NotificationsScreen({super.key});

  @override
  State<NotificationsScreen> createState() =>
      _NotificationsScreenState();
}

class _NotificationsScreenState
    extends State<NotificationsScreen> {
  List<AppNotification> notifications = [];

  bool isLoading = true;

  late NotificationApi api;

  @override
  void initState() {
    super.initState();
    init();
  }

  Future<void> init() async {
    final token = await TokenStorage().getAccessToken();

    api = NotificationApi(
      ApiClient(token).dio,
    );

    await load();
  }

  Future<void> load() async {
    setState(() => isLoading = true);

    try {
      notifications = await api.getAll();
    } catch (_) {}

    if (!mounted) return;

    setState(() => isLoading = false);
  }

  Future<void> markAsRead(AppNotification n) async {
    if (n.isRead) return;

    await api.markAsRead(n.id);

    await load();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("notifications".tr()),
        actions: [
          LanguageButton(
            onChanged: () {
              setState(() {});
            },
          ),
        ],
      ),

      body: isLoading
          ? const Center(
              child: CircularProgressIndicator(),
            )
          : notifications.isEmpty
              ? Center(
                  child: Text(
                    "no_notifications".tr(),
                  ),
                )
              : ListView.builder(
                  itemCount: notifications.length,
                  itemBuilder: (_, i) {
                    final n = notifications[i];

                    return Card(
                      color: n.isRead
                          ? Colors.white
                          : const Color(0xFFE3F2FD),

                      margin: const EdgeInsets.symmetric(
                        horizontal: 12,
                        vertical: 6,
                      ),

                      child: ListTile(
                        title: Text(
                          n.title,
                          style: TextStyle(
                            fontWeight: n.isRead
                                ? FontWeight.normal
                                : FontWeight.bold,
                          ),
                        ),

                        subtitle: Column(
                          crossAxisAlignment:
                              CrossAxisAlignment.start,
                          children: [
                            const SizedBox(height: 6),

                            Text(n.content),

                            const SizedBox(height: 8),

                            Text(
                              n.createdAt.toString(),
                              style: const TextStyle(
                                fontSize: 12,
                                color: Colors.grey,
                              ),
                            ),
                          ],
                        ),

                        onTap: () => markAsRead(n),
                      ),
                    );
                  },
                ),
    );
  }
}