# ==================================================
# 导入库
# ==================================================
import copy
import json
import threading
from typing import Dict, List, Any
from pathlib import Path

from loguru import logger

from app.tools.path_utils import get_path


_history_cache_lock = threading.RLock()
_history_data_cache: dict[str, tuple[tuple[int, int] | None, Dict[str, Any]]] = {}
_history_name_cache: dict[str, tuple[tuple[int, int] | None, List[str]]] = {}


def _get_file_signature(file_path: Path) -> tuple[int, int] | None:
    try:
        stat_result = file_path.stat()
        return stat_result.st_mtime_ns, stat_result.st_size
    except OSError:
        return None


def _get_cached_history_names(history_dir: Path) -> List[str]:
    cache_key = str(history_dir)
    signature = _get_file_signature(history_dir)

    with _history_cache_lock:
        cached = _history_name_cache.get(cache_key)
        if cached and cached[0] == signature:
            return list(cached[1])

    names = sorted(file.stem for file in history_dir.glob("*.json"))
    with _history_cache_lock:
        _history_name_cache[cache_key] = (signature, list(names))
    return list(names)


# ==================================================
# 历史记录文件路径处理函数
# ==================================================
def get_history_file_path(history_type: str, file_name: str) -> Path:
    """获取历史记录文件路径

    Args:
        history_type: 历史记录类型 (roll_call, lottery 等)
        file_name: 文件名（不含扩展名）

    Returns:
        Path: 历史记录文件路径
    """
    history_dir = get_path(f"data/history/{history_type}_history")
    history_dir.mkdir(parents=True, exist_ok=True)
    return history_dir / f"{file_name}.json"


# ==================================================
# 历史记录数据读写函数
# ==================================================


def load_history_data(history_type: str, file_name: str) -> Dict[str, Any]:
    """加载历史记录数据

    Args:
        history_type: 历史记录类型 (roll_call, lottery 等)
        file_name: 文件名（不含扩展名）

    Returns:
        Dict[str, Any]: 历史记录数据
    """
    file_path = get_history_file_path(history_type, file_name)

    if not file_path.exists():
        return {}

    try:
        cache_key = str(file_path)
        signature = _get_file_signature(file_path)

        with _history_cache_lock:
            cached = _history_data_cache.get(cache_key)
            if cached and cached[0] == signature:
                return copy.deepcopy(cached[1])

        with open(file_path, "r", encoding="utf-8") as f:
            history_data = json.load(f)

        if not isinstance(history_data, dict):
            history_data = {}

        with _history_cache_lock:
            _history_data_cache[cache_key] = (signature, copy.deepcopy(history_data))

        return copy.deepcopy(history_data)
    except Exception as e:
        logger.error(f"加载历史记录数据失败: {e}")
        return {}


def save_history_data(history_type: str, file_name: str, data: Dict[str, Any]) -> bool:
    """保存历史记录数据

    Args:
        history_type: 历史记录类型 (roll_call, lottery 等)
        file_name: 文件名（不含扩展名）
        data: 要保存的数据

    Returns:
        bool: 保存是否成功
    """
    file_path = get_history_file_path(history_type, file_name)
    try:
        with open(file_path, "w", encoding="utf-8") as f:
            json.dump(data, f, ensure_ascii=False, indent=4)
        cache_key = str(file_path)
        with _history_cache_lock:
            _history_data_cache[cache_key] = (
                _get_file_signature(file_path),
                copy.deepcopy(data),
            )
        return True
    except Exception as e:
        logger.error(f"保存历史记录数据失败: {e}")
    return False


def get_all_history_names(history_type: str) -> List[str]:
    """获取所有历史记录名称列表

    Args:
        history_type: 历史记录类型 (roll_call, lottery 等)

    Returns:
        List[str]: 历史记录名称列表
    """
    try:
        history_dir = get_path(f"data/history/{history_type}_history")
        if not history_dir.exists():
            return []
        return _get_cached_history_names(history_dir)
    except Exception as e:
        logger.error(f"获取历史记录名称列表失败: {e}")
        return []
