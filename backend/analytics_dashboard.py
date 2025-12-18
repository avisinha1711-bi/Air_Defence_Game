# backend/analytics_dashboard.py
import pandas as pd
import matplotlib.pyplot as plt
from datetime import datetime, timedelta
import json

class GameAnalytics:
    """Comprehensive analytics for player behavior and game performance"""
    
    def generate_daily_report(self):
        """Generate daily performance report"""
        report = {
            'date': datetime.now().strftime('%Y-%m-%d'),
            'metrics': self._calculate_daily_metrics(),
            'top_players': self._get_top_performers(),
            'trends': self._identify_trends(),
            'recommendations': self._generate_recommendations()
        }
        
        # Create visualizations
        self._create_engagement_chart()
        self._create_skill_distribution()
        self._create_retention_funnel()
        
        return report
    
    def _calculate_daily_metrics(self):
        """Calculate key performance indicators"""
        return {
            'active_players': self._count_active_players(),
            'total_games_played': self._count_games_played(),
            'average_session_time': self._average_session_duration(),
            'player_retention_rate': self._calculate_retention(),
            'monetization_rate': self._calculate_monetization(),
            'average_score': self._average_player_score(),
            'most_common_failure_point': self._find_failure_points()
        }
    
    def player_heatmap(self, player_id):
        """Create heatmap of player accuracy across screen"""
        shots_data = self._get_player_shots(player_id)
        
        # Create 10x10 grid of screen
        heatmap = np.zeros((10, 10))
        
        for shot in shots_data:
            x_bin = min(int(shot['x'] / 10), 9)
            y_bin = min(int(shot['y'] / 10), 9)
            heatmap[x_bin][y_bin] += 1 if shot['hit'] else -0.5
        
        return {
            'heatmap': heatmap.tolist(),
            'weak_spots': self._identify_weak_spots(heatmap),
            'improvement_tips': self._generate_tips_from_heatmap(heatmap)
        }
    
    def predict_player_churn(self, player_id):
        """Predict if player is likely to stop playing"""
        player_data = self._get_player_history(player_id)
        
        features = [
            player_data['days_since_last_play'],
            player_data['session_length_decrease'],
            player_data['score_decrease_rate'],
            player_data['rage_quit_count'],
            player_data['social_interactions']
        ]
        
        # Simple ML model for churn prediction
        churn_probability = self._churn_model.predict_proba([features])[0][1]
        
        return {
            'churn_risk': churn_probability,
            'reasons': self._identify_churn_reasons(player_data),
            'interventions': self._suggest_interventions(churn_probability)
        }
