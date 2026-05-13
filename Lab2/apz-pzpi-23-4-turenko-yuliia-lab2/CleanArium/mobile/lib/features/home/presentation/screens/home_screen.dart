import 'package:flutter/material.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/utils/token_storage.dart';
import '../../../aquarium/data/datasources/aquarium_api.dart';
import '../../../aquarium/data/models/aquarium_model.dart';
import '../../../aquarium/presentation/screens/aquarium_details_screen.dart';
import '../../../aquarium/presentation/widgets/aquarium_card.dart';
import '../../../../shared/widgets/language_button.dart';
import '../../../../shared/widgets/notification_button.dart';
import 'package:easy_localization/easy_localization.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  List<Aquarium> aquariums = [];
  bool isLoading = true;

  late AquariumApi api;

  @override
  void initState() {
    super.initState();
    init();
  }

  Future<void> init() async {
    final token = await TokenStorage().getAccessToken();
    final client = ApiClient(token).dio;

    api = AquariumApi(client);

    await loadAquariums();
  }

  Future<void> loadAquariums() async {
    setState(() => isLoading = true);

    try {
      aquariums = await api.getAll();
    } catch (e) {
      debugPrint(e.toString());
    }

    setState(() => isLoading = false);
  }

  void createAquarium() async {
    final nameController = TextEditingController();
    final locationController = TextEditingController();

    await showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text("create_aquarium".tr()),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(
              controller: nameController,
              decoration: InputDecoration(labelText: "aquarium_name".tr()),
            ),
            TextField(
              controller: locationController,
              decoration: InputDecoration(labelText: "aquarium_location".tr()),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: Text("action_cancel".tr()),
          ),
          ElevatedButton(
            onPressed: () async {
              await api.create(nameController.text, locationController.text);
              if (!mounted) return;

              Navigator.pop(context);
              loadAquariums();
            },
            child: Text("action_create".tr()),
          ),
        ],
      ),
    );
  }

  void editAquarium(Aquarium a) async {
    final nameController = TextEditingController(text: a.name);
    final locationController = TextEditingController(text: a.location);

    await showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text("aquarium_edit_title".tr()),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(controller: nameController),
            TextField(controller: locationController),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: Text("action_cancel".tr()),
          ),
          ElevatedButton(
            onPressed: () async {
              await api.update(
                a.id,
                nameController.text,
                locationController.text,
              );
              if (!mounted) return;
              Navigator.pop(context);
              loadAquariums();
            },
            child: Text("action_save".tr()),
          ),
        ],
      ),
    );
  }

  void deleteAquarium(int id) async {
    await showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text("delete_confirm".tr()),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: Text("action_cancel".tr()),
          ),
          ElevatedButton(
            onPressed: () async {
              await api.delete(id);
              if (!mounted) return;
              Navigator.pop(context);
              loadAquariums();
            },
            child: Text("action_delete".tr()),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("CleanArium"),
        actions: [
          IconButton(icon: const Icon(Icons.support_agent), onPressed: () {}),
          NotificationButton(),
          LanguageButton(
            onChanged: () {
              setState(() {});
            },
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: createAquarium,
        child: const Icon(Icons.add),
      ),
      body: isLoading
          ? const Center(child: CircularProgressIndicator())
          : aquariums.isEmpty
          ? Center(child: Text("aquarium_create_first".tr()))
          : ListView.builder(
              itemCount: aquariums.length,
              itemBuilder: (_, i) {
                final a = aquariums[i];

                return AquariumCard(
                  aquarium: a,
                  onTap: () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => AquariumDetailsScreen(
                          aquariumId: a.id,
                          name: a.name,
                        ),
                      ),
                    );
                  },
                  onEdit: () => editAquarium(a),
                  onDelete: () => deleteAquarium(a.id),
                );
              },
            ),
    );
  }
}
