"""History package exports.

Keep package imports lazy so submodules can be imported independently without
triggering circular imports during startup.
"""

from importlib import import_module


_EXPORTS = {
    "get_history_file_path": "app.common.history.file_utils",
    "load_history_data": "app.common.history.file_utils",
    "save_history_data": "app.common.history.file_utils",
    "get_all_history_names": "app.common.history.file_utils",
    "get_name_history": "app.common.history.statistics",
    "get_draw_sessions_history": "app.common.history.statistics",
    "get_individual_statistics": "app.common.history.statistics",
    "save_lottery_history": "app.common.history.lottery_history",
    "save_roll_call_history": "app.common.history.roll_call_history",
    "format_weight_for_display": "app.common.history.weight_utils",
    "calculate_weight": "app.common.history.weight_utils",
    "get_all_names": "app.common.history.utils",
    "format_table_item": "app.common.history.utils",
    "create_table_item": "app.common.history.utils",
    "get_roll_call_student_list": "app.common.history.history_reader",
    "get_roll_call_history_data": "app.common.history.history_reader",
    "filter_roll_call_history_by_subject": "app.common.history.history_reader",
    "get_roll_call_student_total_count": "app.common.history.history_reader",
    "get_roll_call_students_data": "app.common.history.history_reader",
    "get_roll_call_session_data": "app.common.history.history_reader",
    "get_roll_call_student_stats_data": "app.common.history.history_reader",
    "check_class_has_gender_or_group": "app.common.history.history_reader",
    "get_lottery_pool_list": "app.common.history.history_reader",
    "get_lottery_history_data": "app.common.history.history_reader",
    "get_lottery_prizes_data": "app.common.history.history_reader",
    "get_lottery_session_data": "app.common.history.history_reader",
    "get_lottery_prize_stats_data": "app.common.history.history_reader",
}

__all__ = list(_EXPORTS)


def __getattr__(name: str):
    module_name = _EXPORTS.get(name)
    if module_name is None:
        raise AttributeError(f"module {__name__!r} has no attribute {name!r}")
    module = import_module(module_name)
    value = getattr(module, name)
    globals()[name] = value
    return value


def __dir__():
    return sorted(list(globals().keys()) + __all__)
