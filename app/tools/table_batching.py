from collections.abc import Callable, Sequence

from PySide6.QtCore import QTimer


def next_request_id(owner, attr_name: str = "_populate_request_id") -> int:
    request_id = int(getattr(owner, attr_name, 0)) + 1
    setattr(owner, attr_name, request_id)
    return request_id


def run_batched(
    owner,
    request_id: int,
    items: Sequence,
    render_batch: Callable[[int, Sequence, int, int], None],
    *,
    batch_size: int = 30,
    on_finish: Callable[[int], None] | None = None,
    delay_ms: int = 0,
    request_attr: str = "_populate_request_id",
) -> None:
    def _step(start_index: int = 0) -> None:
        if int(getattr(owner, request_attr, 0)) != request_id:
            return

        end_index = min(start_index + batch_size, len(items))
        render_batch(request_id, items, start_index, end_index)

        if end_index < len(items):
            QTimer.singleShot(delay_ms, lambda offset=end_index: _step(offset))
            return

        if on_finish is not None:
            on_finish(request_id)

    _step(0)
