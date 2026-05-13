import 'package:flutter/material.dart';
import '../../data/models/aquarium_model.dart';

class AquariumCard extends StatelessWidget {
  final Aquarium aquarium;
  final VoidCallback onTap;
  final VoidCallback onEdit;
  final VoidCallback onDelete;

  const AquariumCard({
    super.key,
    required this.aquarium,
    required this.onTap,
    required this.onEdit,
    required this.onDelete,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Card(
        margin: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Row(
            children: [
              // ЛІВА ЧАСТИНА
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      aquarium.name,
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 6),
                    Text(
                      aquarium.location ?? "No location",
                      style: const TextStyle(color: Colors.black54),
                    ),
                    const SizedBox(height: 6),
                    Text(
                      aquarium.isActive ? "Active" : "Inactive",
                      style: TextStyle(
                        color: aquarium.isActive
                            ? Colors.green
                            : Colors.red,
                      ),
                    ),
                  ],
                ),
              ),

              // ПРАВА ЧАСТИНА
              Column(
                children: [
                  IconButton(
                    icon: const Icon(Icons.edit),
                    onPressed: onEdit,
                  ),
                  IconButton(
                    icon: const Icon(Icons.delete),
                    onPressed: onDelete,
                  ),
                ],
              )
            ],
          ),
        ),
      ),
    );
  }
}