import sys

import bpy


def main():
    args = sys.argv[sys.argv.index("--") + 1 :]
    output_path = args[0]

    scene = bpy.context.scene
    armature = bpy.data.objects["Fox_Armature"]
    mesh = bpy.data.objects["Fox"]
    source_actions = list(bpy.data.actions)
    baked_actions = []

    armature.animation_data_create()
    for track in armature.animation_data.nla_tracks:
        track.mute = True

    for source_action in source_actions:
        frame_start = int(source_action.frame_range[0])
        frame_end = int(source_action.frame_range[1])
        sampled_pose = []

        armature.animation_data.action = source_action
        if hasattr(armature.animation_data, "action_slot") and len(source_action.slots) > 0:
            armature.animation_data.action_slot = source_action.slots[0]

        for frame in range(frame_start, frame_end + 1):
            scene.frame_set(frame)
            bpy.context.view_layer.update()
            sampled_pose.append(
                [
                    (
                        bone.name,
                        bone.location.copy(),
                        bone.rotation_quaternion.copy(),
                        bone.scale.copy(),
                    )
                    for bone in armature.pose.bones
                ]
            )

        baked_action = bpy.data.actions.new(source_action.name + "_BAKED_TEMP")
        baked_actions.append((source_action.name, baked_action))
        armature.animation_data.action = baked_action
        if hasattr(armature.animation_data, "action_slot") and len(baked_action.slots) > 0:
            armature.animation_data.action_slot = baked_action.slots[0]

        for frame_offset, pose in enumerate(sampled_pose):
            frame = frame_start + frame_offset
            for bone_name, location, rotation, scale in pose:
                bone = armature.pose.bones[bone_name]
                bone.location = location
                bone.rotation_quaternion = rotation
                bone.scale = scale
                bone.keyframe_insert(data_path="location", frame=frame)
                bone.keyframe_insert(data_path="rotation_quaternion", frame=frame)
                bone.keyframe_insert(data_path="scale", frame=frame)

    for source_action in source_actions:
        bpy.data.actions.remove(source_action)

    for original_name, baked_action in baked_actions:
        baked_action.name = original_name

    armature.animation_data.action = baked_actions[0][1]
    bpy.ops.object.select_all(action="DESELECT")
    armature.select_set(True)
    mesh.select_set(True)
    bpy.context.view_layer.objects.active = armature

    bpy.ops.export_scene.fbx(
        filepath=output_path,
        use_selection=True,
        object_types={"ARMATURE", "MESH"},
        add_leaf_bones=False,
        bake_anim=True,
        bake_anim_use_all_actions=True,
        bake_anim_use_nla_strips=False,
        bake_anim_force_startend_keying=True,
        bake_anim_simplify_factor=0.0,
        path_mode="AUTO",
        embed_textures=False,
        axis_forward="-Z",
        axis_up="Y",
        apply_unit_scale=True,
    )

    print("EXPORTED_BAKED_OPEN_GAME_ART_FOX", output_path, [action.name for _, action in baked_actions])


if __name__ == "__main__":
    main()
