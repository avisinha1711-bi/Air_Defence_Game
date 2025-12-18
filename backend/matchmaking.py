# backend/matchmaking.py
from collections import defaultdict
import asyncio

class IntelligentMatchmaking:
    """Smart matchmaking for multiplayer games"""
    
    def __init__(self):
        self.players_queue = defaultdict(list)
        self.skill_cache = {}
        
    async def find_opponents(self, player_id, player_skill):
        """Find suitable opponents based on skill, ping, and preferences"""
        
        # Calculate optimal opponents
        candidates = self._get_candidates(player_skill)
        
        # Score each candidate
        scored_candidates = []
        for candidate in candidates:
            score = self._calculate_match_score(
                player_skill, 
                candidate['skill'],
                self._get_ping(player_id, candidate['id']),
                candidate['preferences']
            )
            scored_candidates.append((candidate, score))
        
        # Select best matches
        scored_candidates.sort(key=lambda x: x[1], reverse=True)
        selected = scored_candidates[:3]  # Top 3 matches
        
        return [candidate for candidate, _ in selected]
    
    def _calculate_match_score(self, skill1, skill2, ping, preferences):
        """Calculate how good a match is"""
        skill_diff = abs(skill1 - skill2)
        ping_score = max(0, 100 - ping) / 100  # Lower ping better
        
        # Preference matching
        pref_score = 0.5  # Default
        if preferences.get('competitive') == (skill1 > 1500):
            pref_score = 0.8
        if preferences.get('region') == self._get_region(skill1):
            pref_score += 0.2
        
        # Weighted final score
        return (0.5 * (1 - skill_diff/1000) + 
                0.3 * ping_score + 
                0.2 * pref_score)
    
    def create_tournament(self, players):
        """Create tournament bracket"""
        # Sort by skill
        sorted_players = sorted(players, key=lambda x: x['skill'], reverse=True)
        
        # Swiss-system pairing for first round
        brackets = self._swiss_pairing(sorted_players)
        
        return {
            'rounds': brackets,
            'schedule': self._generate_schedule(brackets),
            'prizes': self._calculate_prizes(len(players))
        }
