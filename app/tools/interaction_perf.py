import time
from dataclasses import dataclass

from loguru import logger


@dataclass(slots=True)
class InteractionTrace:
    label: str
    started_at: float

    def log(self, phase: str) -> None:
        now = time.perf_counter()
        elapsed = now - self.started_at
        logger.debug(f"[interaction] {self.label} | {phase} | {elapsed:.3f}s")


def start_interaction(label: str) -> InteractionTrace:
    trace = InteractionTrace(label=label, started_at=time.perf_counter())
    trace.log("received")
    return trace
